using Crime_Management_System.Models;
using Microsoft.EntityFrameworkCore;
using Crime_Management_System.Repos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crime_Management_System.Data;

namespace Crime_Management_System.Repositories.Implementations
{
    public class CaseRepository : GenericRepo<Case>, ICaseRepository
    {
        public CaseRepository(CrimeDbContext context) : base(context)
        {
        }

        public async Task<Case?> GetByCaseNumberAsync(string caseNumber)
        {
            return await _table
                .Include(c => c.CreatedByUser)
                .FirstOrDefaultAsync(c => c.CaseNumber == caseNumber);
        }

        public async Task<IEnumerable<Case>> GetCasesByUserAsync(int userId)
        {
            return await _table
                .Include(c => c.CreatedByUser)
                .Where(c => c.CreatedByUserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Case>> GetAssignedCasesAsync(int officerId)
        {
            return await _table
                .Include(c => c.CaseAssignees)
                .ThenInclude(ca => ca.User)
                .Where(c => c.CaseAssignees.Any(a => a.UserId == officerId))
                .ToListAsync();
        }

        public async Task<bool> CaseNumberExistsAsync(string caseNumber)
        {
            return await _table.AnyAsync(c => c.CaseNumber == caseNumber);
        }

        public Task<Case> CreateAsync(Case caseEntity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Case> UpdateAsync(Case caseEntity)
        {
            throw new NotImplementedException();
        }
    }
}
