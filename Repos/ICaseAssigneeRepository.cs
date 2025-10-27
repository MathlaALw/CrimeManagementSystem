using Crime_Management_System.Models;

namespace Crime_Management_System.Repos
{
    public interface ICaseAssigneeRepository
    {
        Task<CaseAssignee> CreateAsync(CaseAssignee assignment);
        Task<bool> DeleteAsync(int id);
        Task<int> GetAssigneeCountAsync(int caseId);
        Task<IEnumerable<CaseAssignee>> GetByCaseIdAsync(int caseId);
        Task<CaseAssignee> GetByIdAsync(int id);
        Task<IEnumerable<CaseAssignee>> GetByOfficerIdAsync(int officerId);
        Task<bool> OfficerAssignedToCaseAsync(int caseId, int officerId);
    }
}