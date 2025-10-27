using Crime_Management_System.Models;


namespace CrimeManagementSystem.Services.Interfaces
{
    public interface ICaseService
    {
        Task<IEnumerable<Case>> GetAllCasesAsync();
        Task<Case> GetCaseByIdAsync(int id);
        Task<Case> CreateCaseAsync(Case caseEntity);
        Task<Case> UpdateCaseAsync(Case caseEntity);
        Task<bool> DeleteCaseAsync(int id);
        Task<IEnumerable<Case>> GetCasesByUserAsync(int userId);
        Task<IEnumerable<Case>> GetAssignedCasesAsync(int officerId);
    }
}
