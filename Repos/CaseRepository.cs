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

        public async Task<IEnumerable<Case>> GetCasesWithDetailsAsync()
        {
            return await _context.Cases
                .Include(c => c.CaseParticipants)
                .Include(c => c.CaseAssignees)
                .Include(c => c.Evidences)
                .Include(c => c.CaseReports)
                .Include(c => c.CreatedByUser)
                .ToListAsync();
        }

        public async Task<Case?> GetCaseWithDetailsByIdAsync(int id)
        {
            return await _context.Cases
                .Include(c => c.CaseParticipants)
                .Include(c => c.CaseAssignees)
                .Include(c => c.Evidences)
                .Include(c => c.CaseReports)
                .Include(c => c.CreatedByUser)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
