using Crime_Management_System.Models;

namespace Crime_Management_System.Repos
{
    public interface ICrimeReportRepository
    {
        Task<CrimeReport> CreateAsync(CrimeReport report);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<CrimeReport>> GetAllAsync();
        Task<CrimeReport> GetByIdAsync(int id);
        Task<CrimeReport> GetByReportIdAsync(int reportId);
        Task<IEnumerable<CrimeReport>> GetPendingReportsAsync();
        Task<IEnumerable<CrimeReport>> GetReportsByCaseAsync(int caseId);
        Task<CrimeReport> UpdateAsync(CrimeReport report);
    }
}