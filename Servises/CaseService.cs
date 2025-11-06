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
            if (id <= 0)
                throw new ArgumentException("Invalid ID. The ID must be greater than zero.");

            try
            {
                var caseEntity = await _caseRepository.GetCaseWithDetailsByIdAsync(id);

                if (caseEntity == null)
                    return null;

                return caseEntity;
            }
            catch (DbUpdateException ex)
            {

                throw new InvalidOperationException("Database error occurred while retrieving the case.", ex);
            }
            catch (Exception ex)
            {

                throw new Exception("An unexpected error occurred in GetCaseByIdAsync.", ex);
            }
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
        public async Task DeleteCaseAsync(int id)
        {
            var caseItem = await _dbContext.Cases
                .Include(c => c.CaseReports)
                .Include(c => c.CaseParticipants)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (caseItem == null)
                throw new InvalidOperationException("Case not found.");


            _dbContext.Cases.Remove(caseItem);
            await _dbContext.SaveChangesAsync();
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
        public async Task<Case>UpdateCaseAsync(int id, UpdateCaseDto caseEntity)
        {
            if (caseEntity == null)
                throw new ArgumentException("Case data cannot be null.");

            var existingCase = await _dbContext.Cases.FindAsync(id);
            if (existingCase == null)
                throw new InvalidOperationException("Case not found.");

            if (string.IsNullOrWhiteSpace(caseEntity.Name))
                throw new ArgumentException("Case name is required.");

            if (string.IsNullOrWhiteSpace(caseEntity.Description))
                throw new ArgumentException("Case description is required.");

            if (string.IsNullOrWhiteSpace(caseEntity.AreaCity))
                throw new ArgumentException("Case area/city is required.");

            if (string.IsNullOrWhiteSpace(caseEntity.CaseType))
                throw new ArgumentException("Case type is required.");

            existingCase.Name = caseEntity.Name;
            existingCase.Description = caseEntity.Description;
            existingCase.AreaCity = caseEntity.AreaCity;
            existingCase.CaseType = caseEntity.CaseType;
            existingCase.Status = caseEntity.Status;


            await _dbContext.SaveChangesAsync();
            return existingCase;
        }

        public async Task<CaseDetailsDto?> GetCaseDetailsAsync(int id)
        {
            // 1) Load the case itself
            var caseEntity = await _dbContext.Cases
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
            if (caseEntity == null)
                return null;
            // 2) CreatedBy (username / fullname)
            var createdByName = await _dbContext.Users
                .Where(u => u.Id == caseEntity.CreatedByUserId)
                .Select(u => u.FullName ?? u.Username)
                .FirstOrDefaultAsync();
            // 3) ReportedBy (citizen/admin/investigator through linked crime reports)
            //    Adjust join/table names if yours are slightly different.
            string? reportedBy = null;
            var reporterNames = await (
                from cr in _dbContext.CrimeReports
                join crp in _dbContext.CaseReports on cr.Id equals crp.ReportId
                join u in _dbContext.Users on cr.ReportedByUserId equals u.Id into userGroup
                from u in userGroup.DefaultIfEmpty()
                where crp.CaseId == id
                select u != null
                    ? (u.FullName ?? u.Username)
                    : "Anonymous"
            )
            .Distinct()
            .ToListAsync();
            if (reporterNames.Count == 1)
                reportedBy = reporterNames[0];
            else if (reporterNames.Count > 1)
                reportedBy = string.Join(", ", reporterNames);
            // 4) Counts
            var numberOfAssignees = await _dbContext.CaseAssignees
                .CountAsync(a => a.CaseId == id);
            // 
            var numberOfEvidences = await _dbContext.Evidences
                .CountAsync(e => e.CaseId == id && !e.IsSoftDeleted);
            //  Count suspects
            var numberOfSuspects = await _dbContext.CaseParticipants
                .Where(cp => cp.CaseId == id && cp.Role == ParticipantRole.Suspect)
                .CountAsync();
            // Count victims
            var numberOfVictims = await _dbContext.CaseParticipants
                .Where(cp => cp.CaseId == id && cp.Role == ParticipantRole.Victim)
                .CountAsync();
            // Count witnesses
            var numberOfWitnesses = await _dbContext.CaseParticipants
                .Where(cp => cp.CaseId == id && cp.Role == ParticipantRole.Witness)
                .CountAsync();
            // 5) Map to DTO
            return new CaseDetailsDto
            {
                Id = caseEntity.Id,
                CaseNumber = caseEntity.CaseNumber,
                Name = caseEntity.Name,
                Description = caseEntity.Description,
                AreaCity = caseEntity.AreaCity,
                CreatedBy = createdByName,
                CreatedAt = caseEntity.CreatedAt,
                CaseType = caseEntity.CaseType,
                Status = caseEntity.Status,
                AuthorizationLevel = caseEntity.AuthorizationLevel,
                ReportedBy = reportedBy,
                Assignees = numberOfAssignees,
                Evidences = numberOfEvidences,
                Suspects = numberOfSuspects,
                Victims = numberOfVictims,
                Witnesses = numberOfWitnesses
            };
        
    
}

}
}

