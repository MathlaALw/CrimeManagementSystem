using Crime_Management_System.DTOs;
using Crime_Management_System.Models;

namespace Crime_Management_System.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<User> CreateUserAsync(CreateUserDto createUserDto, string createdByAdmin);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
        Task AssignRoleAndClearanceAsync(int id, UserRole role, int clearanceLevel);

        Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail);
        bool ValidatePassword(string password, string passwordHash);
    }
}
