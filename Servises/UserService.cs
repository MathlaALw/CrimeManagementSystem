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



        public async Task<List<UserResponseDto>> GetAllUsersAsync()
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

        public async Task<UserResponseDto> GetUserByIdAsync(int userId)
        {
            var user = await _db.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Role = u.Role.ToString(),
                    ClearanceLevel = u.ClearanceLevel.ToString(),
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }

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

        public async Task<UserResponseDto> UpdateUserAsync(int userId, UpdateUserDto updateUserDto, string updatedByAdmin)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }

            // Check if username is being changed and if it already exists
            if (!string.IsNullOrEmpty(updateUserDto.FullName) &&
               updateUserDto.FullName != user.FullName)
            {
                user.FullName = updateUserDto.FullName;
            }

            

            // Update properties if provided
            if (!string.IsNullOrEmpty(updateUserDto.FullName))
                user.Username = updateUserDto.FullName;

            if (!string.IsNullOrEmpty(updateUserDto.Password))
            {
                user.PasswordHash = HashPassword(updateUserDto.Password);
            }

            if (!string.IsNullOrEmpty(updateUserDto.Role.ToString()))
            {
                // Validate clearance level for new role
                var clearanceLevel = !string.IsNullOrEmpty(updateUserDto.ClearanceLevel.ToString())
                    ? updateUserDto.ClearanceLevel
                    : user.ClearanceLevel;

                if (updateUserDto.ClearanceLevel.HasValue)
                    user.ClearanceLevel = updateUserDto.ClearanceLevel.Value;


                if (updateUserDto.Role.HasValue)
                    user.Role = updateUserDto.Role.Value;

            }

            if (updateUserDto.ClearanceLevel.HasValue)
                user.ClearanceLevel = updateUserDto.ClearanceLevel.Value;


            //if (updateUserDto.IsActive.HasValue)
            //    user.IsActive = updateUserDto.IsActive.Value;

            user.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            _logger.LogInformation("User {Username} updated by admin {AdminUsername}",
                user.Username, updatedByAdmin);

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

        public async Task<bool> DeleteUserAsync(int userId, string deletedByAdmin)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }

            // Prevent admin from deleting themselves
            if (user.Username == deletedByAdmin)
            {
                throw new InvalidOperationException("Cannot delete your own account");
            }

            // Soft delete (set IsActive to false)
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            _logger.LogInformation("User {Username} deleted by admin {AdminUsername}",
                user.Username, deletedByAdmin);

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
        Task<IEnumerable<User>> IUserService.GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }

        Task<User> IUserService.GetUserByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteUserAsync(int id)
        {
            throw new NotImplementedException();
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

        Task<User> IUserService.CreateUserAsync(CreateUserDto createUserDto, string createdByAdmin)
        {
            throw new NotImplementedException();
        }

        public bool ValidatePassword(string password, string passwordHash)
        {
            throw new NotImplementedException();
        }
    }

}
