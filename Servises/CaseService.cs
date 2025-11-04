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
        private readonly ICaseRepository _caseRepo;
        private readonly IUserRepository _userRepository; // for checking roles & clearance


        public async Task<IEnumerable<CaseDto>> GetAllCasesAsync(string role, int userId)
        {
            var cases = await _caseRepo.GetAllAsync();

            var filtered = role switch
            {
                "Officer" => cases.Where(c => c.CaseAssignees.Any(a => a.UserId == userId)),
                "Investigator" => cases.Where(c => c.CreatedByUserId == userId),
                _ => cases // Admin
            };

            return filtered.Select(MapToDto);
        }

        public async Task<CaseDto?> GetCaseByIdAsync(int id, string role, int userId)
        {
            var caseEntity = await _caseRepo.GetByIdAsync(id);
            if (caseEntity == null) return null;

            if (role == "Officer" && !caseEntity.CaseAssignees.Any(a => a.UserId == userId))
                return null;

            if (role == "Investigator" && caseEntity.CreatedByUserId != userId)
                return null;

            return MapToDto(caseEntity);
        }

        public async Task<CaseDto> CreateCaseAsync(CreateCaseDto dto)
        {
            var newCase = new Case
            {
                CaseNumber = dto.CaseNumber,
                Name = dto.Name,
                Description = dto.Description,
                AreaCity = dto.AreaCity,
                CaseType = dto.CaseType,
                CreatedByUserId = dto.CreatedByUserId,
                Status = CaseStatus.Pending
            };

            await _caseRepo.
            await _caseRepo.SaveChangesAsync();

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



            caseEntity.Name = dto.Name ?? caseEntity.Name;
            caseEntity.Description = dto.Description ?? caseEntity.Description;
            caseEntity.AreaCity = dto.AreaCity ?? caseEntity.AreaCity;
            caseEntity.CaseType = dto.CaseType ?? caseEntity.CaseType;
            caseEntity.Status = dto.Status;

            _caseRepo.Update(caseEntity);
            await _caseRepo.SaveChangesAsync();

            return MapToDto(caseEntity);
        }

        public async Task<bool> DeleteCaseAsync(int id, string role, int userId)
        {
            if (role != "Admin") return false;

            var caseEntity = await _caseRepo.GetByIdAsync(id);
            if (caseEntity == null) return false;

            _caseRepo.Delete(caseEntity);
            await _caseRepo.SaveChangesAsync();
            return true;
        }


      
    }
}
