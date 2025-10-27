using Crime_Management_System.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace Crime_Management_System.Repos
{
  
    public class GenericRepo<T> : IGenericRepo<T> where T : class
    {
        protected readonly CrimeDbContext _context;
        protected readonly DbSet<T> _table;

        public GenericRepo(CrimeDbContext context)
        {
            _context = context;
            _table = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id) => await _table.FindAsync(id);
        public async Task<IEnumerable<T>> GetAllAsync() => await _table.ToListAsync();
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
            await _table.Where(predicate).ToListAsync();
        public async Task AddAsync(T entity) => await _table.AddAsync(entity);
        public void Update(T entity) => _table.Update(entity);
        public void Remove(T entity) => _table.Remove(entity);
        public async Task SaveAsync() => await _context.SaveChangesAsync();
    }
}
