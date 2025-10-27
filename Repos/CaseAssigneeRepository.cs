using Crime_Management_System.Data;
using Crime_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Crime_Management_System.Repos
{
    public class CaseAssigneeRepository : ICaseAssigneeRepository
    {
        private readonly CrimeDbContext _context;

        public CaseAssigneeRepository(CrimeDbContext context)
        {
            _context = context;
        }

        // CRUD Operations
        // Get By Case Assignee Id
        public async Task<CaseAssignee> GetByIdAsync(int id)
        {
            return await _context.CaseAssignees
                .Include(ca => ca.Case)
                .Include(ca => ca.UserId)
                .FirstOrDefaultAsync(ca => ca.Id == id);
        }

        // Get By Case Id
        public async Task<IEnumerable<CaseAssignee>> GetByCaseIdAsync(int caseId)
        {
            return await _context.CaseAssignees
                .Where(ca => ca.CaseId == caseId)
                .Include(ca => ca.UserId)
                .ToListAsync();
        }
        // Get By Officer Id
        public async Task<IEnumerable<CaseAssignee>> GetByOfficerIdAsync(int officerId)
        {
            return await _context.CaseAssignees
                .Where(ca => ca.UserId == officerId)
                .Include(ca => ca.Case)
                .ThenInclude(c => c.CreatedByUser)
                .ToListAsync();
        }
        // Create Case Assignee
        public async Task<CaseAssignee> CreateAsync(CaseAssignee assignment)
        {
            _context.CaseAssignees.Add(assignment);
            await _context.SaveChangesAsync();
            return assignment;
        }

        // Delete Case Assignee
        public async Task<bool> DeleteAsync(int id)
        {
            var assignment = await GetByIdAsync(id);
            if (assignment == null) return false;

            _context.CaseAssignees.Remove(assignment);
            await _context.SaveChangesAsync();
            return true;
        }

        // Check if Officer is Assigned to Case
        public async Task<bool> OfficerAssignedToCaseAsync(int caseId, int officerId)
        {
            return await _context.CaseAssignees
                .AnyAsync(ca => ca.CaseId == caseId && ca.UserId == officerId);
        }

        // Get Assignee Count for a Case
        public async Task<int> GetAssigneeCountAsync(int caseId)
        {
            return await _context.CaseAssignees
                .CountAsync(ca => ca.CaseId == caseId);
        }
    }
}
