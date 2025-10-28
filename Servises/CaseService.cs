using Crime_Management_System.Models;
using Crime_Management_System.Repos.Implementations;
using Crime_Management_System.Repositories.Implementations;
using Crime_Management_System.Data;

using Crime_Management_System.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace Crime_Management_System.Services.Implementations
{
    public class CaseService : ICaseService
    {
        private readonly ICaseRepository _caseRepository;
        private readonly CrimeDbContext _dbContext;

        public CaseService(ICaseRepository caseRepository, CrimeDbContext dbContext) // Corrected parameter name
        {
            _caseRepository = caseRepository;
            _dbContext = dbContext; // Corrected to use the parameter name
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

        public async Task<Case> UpdateAsync(Case caseEntity)
        {
            var existingCase = await _dbContext.Cases // Corrected dbContext to _dbContext
                .FirstOrDefaultAsync(c => c.Id == caseEntity.Id);

            if (existingCase == null)
                throw new Exception("Case not found");

            existingCase.CaseNumber = caseEntity.CaseNumber;
            existingCase.Name = caseEntity.Name;
            existingCase.Description = caseEntity.Description;
            existingCase.AreaCity = caseEntity.AreaCity;
            existingCase.CaseType = caseEntity.CaseType;
            existingCase.AuthorizationLevel = caseEntity.AuthorizationLevel;
            existingCase.Status = caseEntity.Status;

            await _dbContext.SaveChangesAsync(); // Corrected dbContext to _dbContext
            return existingCase;
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

        public Task<Case> UpdateCaseAsync(Case caseEntity)
        {
            throw new NotImplementedException();
        }
    }
}
