using Crime_Management_System.Models;
using Crime_Management_System.Repositories.Implementations;
using Crime_Management_System.Services.Interfaces;

using Crime_Management_System.Repos;
using Crime_Management_System.Repos.Implementations;
using Crime_Management_System.DTOs;
using Microsoft.EntityFrameworkCore;
using Crime_Management_System.Data;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;


namespace Crime_Management_System.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly CrimeDbContext _db;
        private ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, CrimeDbContext db, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _db = db;
            _logger = logger;
        }



        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            {
                return await _db.Users
                    .Select(u => new UserResponseDto
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Role = u.Role.ToString(),
                        ClearanceLevel = u.ClearanceLevel.ToString(),
                        IsActive = u.IsActive,
                        CreatedAt = u.CreatedAt
                    })
                    .ToListAsync();
            }
        }


        //public async Task<UserResponseDto> GetUserByIdAsync(int userId)
        //{
        //    var user = await _db.Users
        //        .Where(u => u.Id == userId)
        //        .Select(u => new UserResponseDto
        //        {
        //            Id = u.Id,
        //            Username = u.Username,
        //            Role = u.Role.ToString(),
        //            ClearanceLevel = u.ClearanceLevel.ToString(),
        //            IsActive = u.IsActive,
        //            CreatedAt = u.CreatedAt
        //        })
        //        .FirstOrDefaultAsync();

        //    if (user == null)
        //    {
        //        throw new KeyNotFoundException($"User with ID {userId} not found");
        //    }

        //    return user;
        //}



        public async Task<User?> GetUserByIdAsync(int userId)
        {
            var user = await _db.Users.FindAsync(userId);
            return user; 
        }


        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }



        public async Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto, string createdByAdmin)
        {
            if (await _db.Users.AnyAsync(u => u.Username == createUserDto.Username))
                throw new InvalidOperationException($"Username '{createUserDto.Username}' already exists");

            if (!Enum.TryParse<UserRole>(createUserDto.Role, true, out var role))
                throw new InvalidOperationException($"Invalid role value: {createUserDto.Role}");

            // 1️⃣ Generate a salt
            var salt = GenerateSalt();

            // 2️⃣ Hash the password + salt
            var passwordHash = HashPassword(createUserDto.Password + salt);

            // 3️⃣ Assign both hash and salt to the User object
            var user = new User
            {
                Username = createUserDto.Username,
                FullName = createUserDto.FullName,
                Email = createUserDto.Email,
                PasswordHash = passwordHash,
                Salt = salt,  // must not be null
                Role = role,
                ClearanceLevel = createUserDto.ClearanceLevel,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role.ToString(),
                ClearanceLevel = user.ClearanceLevel.ToString(),
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }



        private string HashPassword(string passwordWithSalt)
        {
            // Simple hashing without salt storage
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(passwordWithSalt);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }


        private bool ValidateClearanceLevelForRole(UserRole role, ClearanceLevel clearanceLevel)
        {
            return role switch
            {
                UserRole.Admin => clearanceLevel >= ClearanceLevel.High,
                UserRole.Officer => clearanceLevel >= ClearanceLevel.Medium,
               // UserRole.Citizen => clearanceLevel == ClearanceLevel.Low,
                _ => false
            };
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            var existingUser = await _db.Users.FindAsync(user.Id);
            if (existingUser == null)
                throw new KeyNotFoundException("User not found");

       // no null 
            if (!string.IsNullOrEmpty(user.Username))
                existingUser.Username = user.Username;

            if (!string.IsNullOrEmpty(user.Email))
                existingUser.Email = user.Email;

            if (!string.IsNullOrEmpty(user.FullName))
                existingUser.FullName = user.FullName;

            if (!string.IsNullOrEmpty(user.PasswordHash))
                existingUser.PasswordHash = user.PasswordHash;

            if (!string.IsNullOrEmpty(user.Salt))
                existingUser.Salt = user.Salt;

            existingUser.Role = user.Role;
            existingUser.ClearanceLevel = user.ClearanceLevel;
            existingUser.IsActive = user.IsActive;
            existingUser.UpdatedAt = DateTime.UtcNow;

            _db.Users.Update(existingUser);
            await _db.SaveChangesAsync();

            return existingUser;
        }


        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
                return false;

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return true;
        }


        public async Task<bool> AssignRoleAndClearanceAsync(int userId, UserRole role, int clearanceLevel)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found.");

            user.Role = role;
            user.ClearanceLevel = (ClearanceLevel)clearanceLevel;
            await _userRepository.UpdateAsync(user);

            return true;
        }

        //

        public async Task AssignRoleAsync(int userId, UserRole role)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found.");

            user.Role = role;
            await _userRepository.UpdateAsync(user);
        }


        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            return await _userRepository.GetUsersByRoleAsync(role);
        }



        // Validate password using BCrypt
        public bool ValidatePassword(string password , string passwordHash, string salt)
        {
            var hashToCheck = Convert.ToBase64String(
         SHA256.HashData(Encoding.UTF8.GetBytes(password + salt))
     );
            return hashToCheck == passwordHash;
        }

        // Create a new user with BCrypt hash
        //public async Task<User> CreateUserAsync(CreateUserDto dto)
        //{
        //    if (await _userRepository.UserExistsAsync(dto.Username))
        //        throw new InvalidOperationException("Username already exists");

        //    string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        //    var user = new User
        //    {
        //        Username = dto.Username,
        //        Email = dto.Email,
        //        PasswordHash = passwordHash,
        //        Role = dto.Role,
        //        ClearanceLevel = dto.ClearanceLevel,
        //        IsActive = true,
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    await _userRepository.CreateAsync(user);

        //    _logger.LogInformation("User {Username} created", user.Username);

        //    return user;
        //}



        Task IUserService.AssignRoleAndClearanceAsync(int id, UserRole role, int clearanceLevel)
        {
            return AssignRoleAndClearanceAsync(id, role, clearanceLevel);
        }
       
        public static string GenerateSalt(int size = 16)
        {
            var saltBytes = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }


        // Get user by username/email (async)
        public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail)
        {
            return await _userRepository.GetByUsernameOrEmailAsync(usernameOrEmail);
        }

      
    }

}
