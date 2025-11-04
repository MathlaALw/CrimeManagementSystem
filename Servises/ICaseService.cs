using Crime_Management_System.DTOs;
using Crime_Management_System.Models;


namespace Crime_Management_System.Services.Interfaces
{
    public interface ICaseService
    {
        Task<IEnumerable<Case>> GetAllCasesAsync();
        Task<Case> GetCaseByIdAsync(int id);
        Task<(int id, string message)?> CreateCaseAsync(CreateCaseDto dto, int creatorUserId);
        Task<Case> UpdateCaseAsync(Case caseEntity);
        Task<bool> DeleteCaseAsync(int id);
        Task<IEnumerable<Case>> GetCasesByUserAsync(int userId);
        Task<IEnumerable<Case>> GetAssignedCasesAsync(int officerId);
    }
}
