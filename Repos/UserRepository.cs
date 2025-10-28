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
        public UserRepository(CrimeDbContext context) : base(context)
        {
        }

        public async Task<User> CreateAsync(User user)
        {
            await _table.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<User> UpdateAsync(User existingUser)
        {
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

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _table.AnyAsync(u => u.Username.ToLower() == username.ToLower());
        }
    }
}
