using Crime_Management_System.Models;
using System.Collections.Generic;

namespace Crime_Management_System.Repositories.Implementations
{
    public interface ICaseRepository
    {
        Task<bool> CaseNumberExistsAsync(string caseNumber);
        Task<Case> CreateAsync(Case caseEntity);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Case>> GetAllAsync();
        Task<IEnumerable<Case>> GetAssignedCasesAsync(int officerId);
        Task<Case?> GetByCaseNumberAsync(string caseNumber);
        Task<Case> GetByIdAsync(int id);
        Task<IEnumerable<Case>> GetCasesByUserAsync(int userId);
        Task<Case> UpdateAsync(Case caseEntity);
    }
}