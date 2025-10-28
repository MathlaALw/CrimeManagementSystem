using Crime_Management_System.Data;
using Crime_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Crime_Management_System.Repos
{
    public class CaseAssigneeRepository : GenericRepo<CaseAssignee>, ICaseAssigneeRepository
    {
        public CaseAssigneeRepository(CrimeDbContext context) : base(context) { }

        public async Task<IEnumerable<CaseAssignee>> GetByCaseIdAsync(int caseId)
        {
            return await _table
                .Include(ca => ca.User)
                .Include(ca => ca.UserId)
                .Where(ca => ca.CaseId == caseId)
                .ToListAsync();
        }

        public async Task<IEnumerable<CaseAssignee>> GetByOfficerIdAsync(int officerId)
        {
            return await _table
                .Include(ca => ca.Case)
                .ThenInclude(c => c.CreatedByUser)
                .Where(ca => ca.UserId == officerId)
                .ToListAsync();
        }

        public async Task<bool> IsOfficerAssignedToCaseAsync(int caseId, int officerId)
        {
            return await _table
                .AnyAsync(ca => ca.CaseId == caseId && ca.UserId == officerId);
        }

        public async Task<int> GetAssigneeCountAsync(int caseId)
        {
            return await _table
                .CountAsync(ca => ca.CaseId == caseId);
        }

        public async Task<CaseAssignee?> GetByCaseAndOfficerAsync(int caseId, int officerId)
        {
            return await _table
                .Include(ca => ca.User)
                .Include(ca => ca.UserId)
                .FirstOrDefaultAsync(ca => ca.CaseId == caseId && ca.UserId == officerId);
        }

        public Task<CaseAssignee> CreateAsync(CaseAssignee assignment)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> OfficerAssignedToCaseAsync(int caseId, int officerId)
        {
            throw new NotImplementedException();
        }
    }
}
