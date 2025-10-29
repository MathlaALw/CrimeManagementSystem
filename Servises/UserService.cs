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
            if (await _userRepository.UserExistsAsync(user.Username))
                throw new Exception("Username already exists.");

            var existingEmail = await _userRepository.GetByEmailAsync(user.Email);
            if (existingEmail != null)
                throw new Exception("Email already exists.");


            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true;

            return await _userRepository.CreateAsync(user);
        }



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
            existingUser.UpdatedAt = DateTime.UtcNow;

            return await _userRepository.UpdateAsync(existingUser);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteAsync(id);
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
    }
}
