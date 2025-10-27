using Crime_Management_System.DTOs;

namespace Crime_Management_System.Servises
{
    public interface IReportService
    {
        Task<(int id, string status)?> GetStatusAsync(int id);
        Task<(int reportId, string message)?> SubmitAsync(SubmitCrimeReportDto dto);
    }
}