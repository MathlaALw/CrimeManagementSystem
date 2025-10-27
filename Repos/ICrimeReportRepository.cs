using Crime_Management_System.Models;

namespace Crime_Management_System.Repos
{
    public interface ICrimeReportRepository : IGenericRepo<CrimeReport>
    {
        Task<CrimeReport?> GetByReportIdAsync(int reportId);
        Task<CrimeReport?> GetReadOnlyAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<CrimeReport>> GetPendingReportsAsync();
        Task<IEnumerable<CrimeReport>> GetReportsByCaseAsync(int caseId);
        Task<IEnumerable<CrimeReport>> GetReportsByUserAsync(int userId);
        Task<IEnumerable<CrimeReport>> SearchReportsAsync(string? searchTerm, string? status);
    }
}