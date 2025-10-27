using Crime_Management_System.Data;
using Crime_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Crime_Management_System.Repos
{
    public class EvidenceRepository : IEvidenceRepository
    {
        private readonly CrimeDbContext _context;

        public EvidenceRepository(CrimeDbContext context)
        {
            _context = context;
        }

        public async Task<Evidence> GetByIdAsync(int id)
        {
            return await _context.Evidences
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Evidence>> GetByCaseIdAsync(int caseId)
        {
            return await _context.Evidences
                .Where(e => e.CaseId == caseId && !e.IsSoftDeleted)
                .ToListAsync();
        }

        public async Task<Evidence> CreateAsync(Evidence evidence)
        {
            _context.Evidences.Add(evidence);
            await _context.SaveChangesAsync();
            return evidence;
        }

        public async Task<Evidence> UpdateAsync(Evidence evidence)
        {
            _context.Evidences.Update(evidence);
            await _context.SaveChangesAsync();
            return evidence;
        }

    }
}
