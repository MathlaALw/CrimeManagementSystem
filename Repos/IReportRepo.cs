using Crime_Management_System.Models;

namespace Crime_Management_System.Repos
{
    public interface IReportRepo : IGenericRepo<CrimeReport>
    {
        Task<CrimeReport?> GetReadOnlyAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<CrimeReport>> GetPendingReportsAsync();
    }
}