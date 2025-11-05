using Crime_Management_System.DTOs;
using Crime_Management_System.Models;

namespace Crime_Management_System.Servises
{
    public interface ICrimeReportService
    {
        Task<IEnumerable<CrimeReport>> GetAllReportsAsync();
        Task<CrimeReport> CreateReportAsync(SubmitCrimeReportDto reportDto);
        Task<IEnumerable<CrimeReport>> GetPendingReportsAsync();
        Task<CrimeReport> GetReportByIdAsync(int id);
        Task<CrimeReport> GetReportByPublicIdAsync(int reportId);
        Task<string> GetReportStatusAsync(int reportId);
        Task<bool> LinkReportToCaseAsync(int reportId, int caseId);
        Task<CrimeReport> UpdateReportAsync(int id, CrimeReportUpdateDto reportDto);
    }
}