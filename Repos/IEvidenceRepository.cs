using Crime_Management_System.Models;

namespace Crime_Management_System.Repos
{
    public interface IEvidenceRepository
    {
        Task AddAsync(Evidence e);
        Task<IEnumerable<Evidence>> GetDeletedEvidenceAsync();
        Task<Evidence?> GetReadOnlyAsync(int id);
        Task<Evidence?> GetWithTrackingAsync(int id);
        Task<IEnumerable<Evidence>> ListByCaseAsync(int caseId);
        Task SaveAsync(Evidence e);
        Task SaveAsync();
    }
}