using Crime_Management_System.Models;


namespace CrimeManagementSystem.Repositories.Interfaces
{
    public interface ICaseRepository
    {
        Task<Case> GetByIdAsync(int id);
        Task<Case> GetByCaseNumberAsync(string caseNumber);
        Task<IEnumerable<Case>> GetAllAsync();
        Task<IEnumerable<Case>> GetCasesByUserAsync(int userId);
        Task<IEnumerable<Case>> GetAssignedCasesAsync(int officerId);
        Task<Case> CreateAsync(Case caseEntity);
        Task<Case> UpdateAsync(Case caseEntity);
        Task<bool> DeleteAsync(int id);
        Task<bool> CaseNumberExistsAsync(string caseNumber);
    }
}
