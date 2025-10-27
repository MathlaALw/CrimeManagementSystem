using Crime_Management_System.Data;
using Crime_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Crime_Management_System.Repos
{
    public class ReportRepo : GenericRepo<CrimeReport>, IReportRepo
    {
        public ReportRepo(CrimeDbContext db) : base(db) { }

        public Task<CrimeReport?> GetReadOnlyAsync(int id) =>
            _table.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);

        public async Task<bool> ExistsAsync(int id) => await _table.AnyAsync(r => r.Id == id);

        public async Task<IEnumerable<CrimeReport>> GetPendingReportsAsync() =>
            await _table.Where(r => r.Status == "pending").ToListAsync();
    }
}
