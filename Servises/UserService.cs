using Crime_Management_System.Models;
using Crime_Management_System.Repositories.Implementations;
using Crime_Management_System.Services.Interfaces;

using Crime_Management_System.Repos;
using Crime_Management_System.Repos.Implementations;


namespace Crime_Management_System.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }

      
        public async Task<User> CreateUserAsync(User user)
        {
            // Check if username already exists
            bool exists = await _userRepository.UserExistsAsync(user.Username);
            if (exists)
                throw new Exception("Username already exists.");

            // Hash the password before saving
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            user.CreatedAt = DateTime.UtcNow;

            return await _userRepository.CreateAsync(user);
        }

        //  Update user
        public async Task<User> UpdateUserAsync(User user)
        {
            var existingUser = await _userRepository.GetByIdAsync(user.Id);
            if (existingUser == null)
                throw new Exception("User not found.");

            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;
            existingUser.Role = user.Role;
            existingUser.ClearanceLevel = user.ClearanceLevel;
            existingUser.IsActive = user.IsActive;

            return await _userRepository.UpdateAsync(existingUser);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteAsync(id);
        }


        public async Task AssignRoleAsync(int userId, UserRole role)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found.");

            user.Role = role;
            await _userRepository.UpdateAsync(user);
        }


        public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
        {
            return await _userRepository.GetUsersByRoleAsync(role.ToString());
        }

 
        public bool ValidatePassword(User user, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        public Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            throw new NotImplementedException();
        }
    }
}
