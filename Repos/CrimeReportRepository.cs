using Crime_Management_System.Data;
using Crime_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Crime_Management_System.Repos
{
    public class CrimeReportRepository : ICrimeReportRepository
    {
        private readonly CrimeDbContext _context;

        public CrimeReportRepository(CrimeDbContext context)
        {
            _context = context;
        }

        // CRUD Operations
        // Git By Crime Report Id
        public async Task<CrimeReport> GetByIdAsync(int id)
        {
            return await _context.CrimeReports.FindAsync(id);
        }

        // Get By Report Id
        public async Task<CrimeReport> GetByReportIdAsync(int reportId)
        {
            return await _context.CrimeReports
                .FirstOrDefaultAsync(r => r.Id == reportId);
        }

        // Get All Crime Reports
        public async Task<IEnumerable<CrimeReport>> GetAllAsync()
        {
            return await _context.CrimeReports.ToListAsync();
        }

        // Create Crime Report
        public async Task<CrimeReport> CreateAsync(CrimeReport report)
        {
            _context.CrimeReports.Add(report);
            await _context.SaveChangesAsync();
            return report;
        }

        // Update Crime Report
        public async Task<CrimeReport> UpdateAsync(CrimeReport report)
        {
            _context.CrimeReports.Update(report);
            await _context.SaveChangesAsync();
            return report;
        }

        // Delete Crime Report
        public async Task<bool> DeleteAsync(int id)
        {
            var report = await GetByIdAsync(id);
            if (report == null) return false;

            _context.CrimeReports.Remove(report);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get Reports By Case Id
        public async Task<IEnumerable<CrimeReport>> GetReportsByCaseAsync(int caseId)
        {
            return await _context.CrimeReports
                .Where(r => r.Id == caseId)
                .ToListAsync();
        }

        // Get Pending Reports
        public async Task<IEnumerable<CrimeReport>> GetPendingReportsAsync()
        {
            return await _context.CrimeReports
                .Where(r => r.Status == "Pending")
                .ToListAsync();
        }
    }
}
