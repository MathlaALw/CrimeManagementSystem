using Crime_Management_System.Models;

using Microsoft.EntityFrameworkCore;
using Crime_Management_System.Repos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crime_Management_System.Data;

namespace Crime_Management_System.Repos.Implementations
{

    public class UserRepository : GenericRepo<User>, IUserRepository
    {
        private readonly CrimeDbContext _context;

        public UserRepository(CrimeDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User> CreateAsync(User user)
        {
            user.CreatedAt = DateTime.UtcNow;

            await _table.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<User?> UpdateAsync(User updatedUser)
        {
            var existingUser = await _table.FindAsync(updatedUser.Id);
            if (existingUser == null)
                return null;

            existingUser.FullName = updatedUser.FullName;
            existingUser.Email = updatedUser.Email;
            existingUser.Username = updatedUser.Username;
            existingUser.Role = updatedUser.Role;
            existingUser.ClearanceLevel = updatedUser.ClearanceLevel;
            existingUser.IsActive = updatedUser.IsActive;
            existingUser.UpdatedAt = DateTime.UtcNow;

            _table.Update(existingUser);
            await _context.SaveChangesAsync();
            return existingUser;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _table.FindAsync(id);
            if (user == null)
                return false;

            _table.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _table
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            return await _table
                .Where(u => u.Role.ToString().ToLower() == role.ToLower())
                .ToListAsync();
        }

        public async Task<bool> AssignRoleAndClearanceAsync(int userId, int role, int clearanceLevel)
        {
            var user = await _table.FindAsync(userId);
            if (user == null)
                return false;

            user.Role = (UserRole)role;
            user.ClearanceLevel = (ClearanceLevel)clearanceLevel;

            _table.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _table.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }
        
        public async Task<bool> UserExistsAsync(string username)
        {
            return await _table.AnyAsync(u => u.Username.ToLower() == username.ToLower());
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _table.OrderByDescending(u => u.CreatedAt).ToListAsync();
        }

        // GetByUsernameOrEmail
        public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail)
        {
            return await _table
                .FirstOrDefaultAsync(u => u.Username.ToLower() == usernameOrEmail.ToLower()
                                        || u.Email.ToLower() == usernameOrEmail.ToLower());
        }


       
    }
}
