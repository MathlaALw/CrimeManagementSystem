using Crime_Management_System.Models;
using System.Collections.Generic;

namespace Crime_Management_System.Repos.Implementations
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
        Task<bool> UserExistsAsync(string username);
        Task<User> CreateAsync(User user);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User> UpdateAsync(User existingUser);
        Task<bool> DeleteAsync(int id);
    }
}