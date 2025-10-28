using Crime_Management_System.Data;
using Crime_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Crime_Management_System.Repos
{
    public class CrimeReportRepository : GenericRepo<CrimeReport>, ICrimeReportRepository
    {
        public CrimeReportRepository(CrimeDbContext context) : base(context) { }

        // Get a crime report by its report ID, including the user who reported it
        public async Task<CrimeReport?> GetByReportIdAsync(int reportId)
        {
            return await _table
                .Include(r => r.ReportedByUser)
                .FirstOrDefaultAsync(r => r.Id == reportId);
        }

        // Get a read-only crime report by its ID, including the user who reported it
        public async Task<CrimeReport?> GetReadOnlyAsync(int id)
        {
            return await _table
                .AsNoTracking()
                .Include(r => r.ReportedByUser)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        // Check if a crime report exists by its ID
        public async Task<bool> ExistsAsync(int id)
        {
            return await _table.AnyAsync(r => r.Id == id);
        }

        // Get all pending crime reports, ordered by report date and time descending
        public async Task<IEnumerable<CrimeReport>> GetPendingReportsAsync()
        {
            return await _table
                .Where(r => r.Status == "pending")
                .OrderByDescending(r => r.ReportDateTime)
                .ToListAsync();
        }

        // Get all crime reports associated with a specific case ID
        public async Task<IEnumerable<CrimeReport>> GetReportsByCaseAsync(int caseId)
        {
            return await _table
                .Where(r => r.CaseReports.Any(cr => cr.CaseId == caseId))
                .Include(r => r.ReportedByUser)
                .ToListAsync();
        }

        // Get all crime reports submitted by a specific user, ordered by report date and time descending

        public async Task<IEnumerable<CrimeReport>> GetReportsByUserAsync(int userId)
        {
            return await _table
                .Where(r => r.ReportedByUserId == userId)
                .OrderByDescending(r => r.ReportDateTime)
                .ToListAsync();
        }

        // Search crime reports by a search term and status, ordered by report date and time descending
        public async Task<IEnumerable<CrimeReport>> SearchReportsAsync(string? searchTerm, string? status)
        {
            var query = _table.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(r =>
                    r.Title.Contains(searchTerm) ||
                    r.Description.Contains(searchTerm) ||
                    r.AreaCity.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(r => r.Status == status);
            }

            return await query
                .OrderByDescending(r => r.ReportDateTime)
                .ToListAsync();
        }

        public Task<CrimeReport> CreateAsync(CrimeReport report)
        {
            throw new NotImplementedException();
        }

        public Task<CrimeReport> UpdateAsync(CrimeReport report)
        {
            throw new NotImplementedException();
        }
    }
}
