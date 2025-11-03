using Crime_Management_System.Models;
using Crime_Management_System.Repos.Implementations;
using Crime_Management_System.Repositories.Implementations;
using Crime_Management_System.Data;
using Crime_Management_System.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Crime_Management_System.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
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

        //public async Task<(int id , string message)?> CreateCaseAsync(CreateCaseDto createCase , int AddedByUserid)
        //{
        //    var c = new Case
        //    {
        //        CaseNumber = createCase.CaseNumber,
        //        Name = createCase.Name,
        //        Description = createCase.Description,
        //        AreaCity = createCase.AreaCity,
        //        CaseType = createCase.CaseType,
        //        Status = createCase.Status
        //    };




        //}

        // Create Case  
        public async Task<(int id , string message)?> CreateCaseAsync(CreateCaseDto createCaseDto, int addedByUserId)
        {

            // check for duplicate case number
            if (await _dbContext.Cases.AnyAsync(c => c.CaseNumber == createCaseDto.CaseNumber))
            {
                return (0, "Case with the same case number already exists");
            }
            var creator = await _dbContext.Users.FindAsync(addedByUserId);
            if (creator == null) return null;

            // Make sure case level is not HIGHER than creator’s clearance
            var finalLevel = createCaseDto.AuthorizationLevel;
            if (finalLevel > creator.ClearanceLevel)
            {
                finalLevel = creator.ClearanceLevel;
            }


            var newCase = new Case
            {
                CaseNumber = createCaseDto.CaseNumber,
                Name = createCaseDto.Name,
                Description = createCaseDto.Description,
                AreaCity = createCaseDto.AreaCity,
                CaseType = createCaseDto.CaseType,
                Status = CaseStatus.Pending,
                AuthorizationLevel = finalLevel,
                CreatedByUserId = addedByUserId,
                CreatedAt = DateTime.UtcNow
            };

            // attach crime reports with caseReport Join table
            if (createCaseDto.CrimeReportIds != null && createCaseDto.CrimeReportIds.Any())
            {
                // Only keep existing reports to avoid FK errors
                var existingReports = await _dbContext.CrimeReports
                    .Where(r => createCaseDto.CrimeReportIds.Contains(r.Id))
                    .ToListAsync();

                if (!existingReports.Any())
                {
                    // No valid report IDs – treat as invalid request
                    return (0, "No valid crime reports found for the provided IDs");

                }

                newCase.CaseReports = existingReports.Select(r => new CaseReport
                {
                    ReportId = r.Id,
                    Case = newCase,
                    LinkedAt = DateTime.UtcNow
                }).ToList();
            }
            // save to db
            await _caseRepository.CreateAsync(newCase);
            return (newCase.Id, "Case created successfully");

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

        public async Task<Case> UpdateCaseAsync(Case caseEntity)
        {
          
            var existingCase = await _caseRepository.GetByIdAsync(caseEntity.Id);
            if (existingCase == null)
                throw new Exception("Case not found");

            
            existingCase.CaseNumber = caseEntity.CaseNumber;
            existingCase.Name = caseEntity.Name;
            existingCase.Description = caseEntity.Description;
            existingCase.AreaCity = caseEntity.AreaCity;
            existingCase.CaseType = caseEntity.CaseType;
            //existingCase.AuthorizationLevel = caseEntity.AuthorizationLevel;
            existingCase.Status = caseEntity.Status;

           
            return await _caseRepository.UpdateAsync(existingCase);
        }


      
    }
}
