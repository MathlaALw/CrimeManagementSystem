using Crime_Management_System.Models;

namespace Crime_Management_System.Repositories.Implementations
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
        Task<bool> UserExistsAsync(string username);
    }
}