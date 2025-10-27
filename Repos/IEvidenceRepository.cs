using Crime_Management_System.Models;

namespace Crime_Management_System.Repos
{
    public interface IEvidenceRepository
    {
        Task<Evidence> CreateAsync(Evidence evidence);
        Task<IEnumerable<Evidence>> GetByCaseIdAsync(int caseId);
        Task<Evidence> GetByIdAsync(int id);
        Task<Evidence> UpdateAsync(Evidence evidence);
    }
}