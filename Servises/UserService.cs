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

            if (!ValidateClearanceLevelForRole(createUserDto.Role, createUserDto.ClearanceLevel))
                throw new InvalidOperationException($"Clearance level '{createUserDto.ClearanceLevel}' is not valid for role '{createUserDto.Role}'");

            
            string salt = GenerateSalt();

        
            string passwordHash = HashPassword(createUserDto.Password + salt);

            var user = new User
            {
                Username = createUserDto.Username,
                PasswordHash = passwordHash,
                Salt = salt,
                Role = createUserDto.Role,
                ClearanceLevel = createUserDto.ClearanceLevel,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            _logger.LogInformation("User {Username} created by admin {AdminUsername}", createUserDto.Username, createdByAdmin);

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



        public bool ValidatePassword(User user, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }



        Task IUserService.AssignRoleAndClearanceAsync(int id, UserRole role, int clearanceLevel)
        {
            return AssignRoleAndClearanceAsync(id, role, clearanceLevel);
        }

        public Task<User> CreateUserAsync(User user)
        {
            throw new NotImplementedException();
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

    }

}
