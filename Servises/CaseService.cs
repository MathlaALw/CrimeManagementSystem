using Crime_Management_System.Models;

using CrimeManagementSystem.Services.Interfaces;

namespace CrimeManagementSystem.Services.Implementations
{
    public class CaseService : ICaseService
    {
        private readonly ICaseRepository _caseRepository;

        public CaseService(ICaseRepository caseRepository)
        {
            _caseRepository = caseRepository;
        }

        public async Task<IEnumerable<Case>> GetAllCasesAsync()
        {
            return await _caseRepository.GetAllAsync();
        }

        public async Task<Case> GetCaseByIdAsync(int id)
        {
            return await _caseRepository.GetByIdAsync(id);
        }

        public async Task<Case> CreateCaseAsync(Case caseEntity)
        {
            if (await _caseRepository.CaseNumberExistsAsync(caseEntity.CaseNumber))
                throw new Exception("Case number already exists");

            return await _caseRepository.CreateAsync(caseEntity);
        }

        public async Task<Case> UpdateCaseAsync(Case caseEntity)
        {
            return await _caseRepository.UpdateAsync(caseEntity);
        }

        public async Task<bool> DeleteCaseAsync(int id)
        {
            return await _caseRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Case>> GetCasesByUserAsync(int userId)
        {
            return await _caseRepository.GetCasesByUserAsync(userId);
        }

        public async Task<IEnumerable<Case>> GetAssignedCasesAsync(int officerId)
        {
            return await _caseRepository.GetAssignedCasesAsync(officerId);
        }
    }
}
