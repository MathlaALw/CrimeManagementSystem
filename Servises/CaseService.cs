using Crime_Management_System.Models;
using Crime_Management_System.Repositories.Implementations;
using Crime_Management_System.Data;
using Crime_Management_System.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Crime_Management_System.DTOs;

namespace Crime_Management_System.Services.Implementations
{
    public class CaseService : ICaseService
    {
        private readonly ICaseRepository _caseRepository;
        private readonly CrimeDbContext _dbContext;

        public CaseService(ICaseRepository caseRepository, CrimeDbContext dbContext)
        {
            _caseRepository = caseRepository;
            _dbContext = dbContext;
        }

        // Get all cases (with details)
        public async Task<IEnumerable<Case>> GetAllCasesAsync()
        {
            return await _caseRepository.GetCasesWithDetailsAsync();
        }

        // Get single case (with details)
        public async Task<Case> GetCaseByIdAsync(int id)
        {
            var caseEntity = await _caseRepository.GetCaseWithDetailsByIdAsync(id);
            if (caseEntity == null)
                throw new Exception("Case not found");

            return caseEntity;
        }

        // Create Case
        public async Task<(int id, string message)?> CreateCaseAsync(CreateCaseDto createCaseDto, int addedByUserId)
        {
            // Check duplicate case number
            if (await _dbContext.Cases.AnyAsync(c => c.CaseNumber == createCaseDto.CaseNumber))
            {
                return (0, "Case with the same case number already exists");
            }

            // Get creator (for clearance level + CreatedByUserId)
            var creator = await _dbContext.Users.FindAsync(addedByUserId);
            if (creator == null) return null;

            // Ensure case's authorization level is not higher than creator clearance
            //var finalLevel = createCaseDto.AuthorizationLevel;
            //if (finalLevel > creator.ClearanceLevel)
            //{
            //    finalLevel = creator.ClearanceLevel;
            //}

            // Build case entity
            var newCase = new Case
            {
                CaseNumber = createCaseDto.CaseNumber,
                Name = createCaseDto.Name,
                Description = createCaseDto.Description,
                AreaCity = createCaseDto.AreaCity,
                CaseType = createCaseDto.CaseType,
                Status = CaseStatus.Pending,
              //  AuthorizationLevel = finalLevel,
                CreatedByUserId = addedByUserId,
                CreatedAt = DateTime.UtcNow
            };

            // Attach crime reports (CaseReports join table)
            if (createCaseDto.CrimeReportIds != null && createCaseDto.CrimeReportIds.Any())
            {
                var existingReports = await _dbContext.CrimeReports
                    .Where(r => createCaseDto.CrimeReportIds.Contains(r.Id))
                    .ToListAsync();

                if (!existingReports.Any())
                {
                    return (0, "No valid crime reports found for the provided IDs");
                }

                newCase.CaseReports = existingReports
                    .Select(r => new CaseReport
                    {
                        ReportId = r.Id,
                        Case = newCase,
                        LinkedAt = DateTime.UtcNow
                    })
                    .ToList();
            }

            // 6) Save via DbContext
            await _dbContext.Cases.AddAsync(newCase);
            await _dbContext.SaveChangesAsync();

            return (newCase.Id, "Case created successfully");
        }

        // Delete a case
        public async Task<bool> DeleteCaseAsync(int id)
        {
            var existing = await _dbContext.Cases.FindAsync(id);
            if (existing == null)
                return false;

            _dbContext.Cases.Remove(existing);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // All cases created by a specific user
        public async Task<IEnumerable<Case>> GetCasesByUserAsync(int userId)
        {
            return await _dbContext.Cases
                .Where(c => c.CreatedByUserId == userId)
                .ToListAsync();
        }

        // All cases assigned to a specific officer
        public async Task<IEnumerable<Case>> GetAssignedCasesAsync(int officerId)
        {
            return await _dbContext.CaseAssignees
                .Include(a => a.Case)
                .Where(a => a.UserId == officerId)
                .Select(a => a.Case!)
                .ToListAsync();
        }

        // Update case
        public async Task<Case> UpdateCaseAsync(Case caseEntity)
        {
            var existingCase = await _dbContext.Cases.FindAsync(caseEntity.Id);
            if (existingCase == null)
                throw new Exception("Case not found");

            existingCase.CaseNumber = caseEntity.CaseNumber;
            existingCase.Name = caseEntity.Name;
            existingCase.Description = caseEntity.Description;
            existingCase.AreaCity = caseEntity.AreaCity;
            existingCase.CaseType = caseEntity.CaseType;
           
            // existingCase.AuthorizationLevel = caseEntity.AuthorizationLevel;
            existingCase.Status = caseEntity.Status;

            await _dbContext.SaveChangesAsync();
            return existingCase;
        }
    }
}
