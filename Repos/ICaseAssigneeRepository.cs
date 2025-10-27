using Crime_Management_System.Models;

namespace Crime_Management_System.Repos
{
    public interface ICaseAssigneeRepository
    {
        Task<int> GetAssigneeCountAsync(int caseId);
        Task<CaseAssignee?> GetByCaseAndOfficerAsync(int caseId, int officerId);
        Task<IEnumerable<CaseAssignee>> GetByCaseIdAsync(int caseId);
        Task<IEnumerable<CaseAssignee>> GetByOfficerIdAsync(int officerId);
        Task<bool> IsOfficerAssignedToCaseAsync(int caseId, int officerId);
    }
}