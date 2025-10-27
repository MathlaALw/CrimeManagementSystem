using Crime_Management_System.Data;
using Crime_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Crime_Management_System.Repos
{
    public class EvidenceRepository : GenericRepo<Evidence>, IEvidenceRepository
    {
        public EvidenceRepository(CrimeDbContext db) : base(db) { }

        public async Task<Evidence?> GetWithTrackingAsync(int id) =>
            await _table.FirstOrDefaultAsync(e => e.Id == id);

        public async Task<Evidence?> GetReadOnlyAsync(int id) =>
            await _table.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);

        public async Task<IEnumerable<Evidence>> ListByCaseAsync(int caseId) =>
            await _table.AsNoTracking()
                .Where(e => e.CaseId == caseId && !e.IsSoftDeleted)
                .OrderByDescending(e => e.Id)
                .ToListAsync();

        public async Task<IEnumerable<Evidence>> GetDeletedEvidenceAsync() =>
            await _table.Where(e => e.IsSoftDeleted).ToListAsync();

        public Task SaveAsync(Evidence e)
        {
            throw new NotImplementedException();
        }
    }
}

