using Crime_Management_System.Models;

namespace Crime_Management_System.Repos
{
    public interface IEvidenceRepository : IGenericRepo<Evidence>
    {
        Task<Evidence?> GetWithTrackingAsync(int id);
        Task<Evidence?> GetReadOnlyAsync(int id);
        Task<IEnumerable<Evidence>> ListByCaseAsync(int caseId);
        Task<IEnumerable<Evidence>> GetDeletedEvidenceAsync();
    }
}