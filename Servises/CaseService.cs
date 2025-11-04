using Crime_Management_System.Models;
using Crime_Management_System.Repos.Implementations;
using Crime_Management_System.Repositories.Implementations;
using Crime_Management_System.Data;
using Crime_Management_System.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Crime_Management_System.DTOs;
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

            return MapToDto(newCase);
        }

        public async Task<CaseDto?> UpdateCaseAsync(int id, UpdateCaseDto dto, string role, int userId)
        {
            var caseEntity = await _caseRepo.GetByIdAsync(id);
            if (caseEntity == null) return null;

            if (role == "Investigator" && caseEntity.CreatedByUserId != userId)
                return null;

            if (role == "Officer" && dto.Status == CaseStatus.Closed)
                return null;

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

        private static CaseDto MapToDto(Case c) => new CaseDto
        {
            Id = c.Id,
            CaseNumber = c.CaseNumber,
            Name = c.Name,
            Description = c.Description,
            AreaCity = c.AreaCity,
            CaseType = c.CaseType,
            Status = c.Status,
            CreatedAt = c.CreatedAt,
            CreatedBy = c.CreatedByUser?.FullName ?? "Unknown",
            CaseParticipants = c.CaseParticipants.Select(p => p.ParticipantName).ToList(),
            CaseAssignees = c.CaseAssignees.Select(a => a.User.FullName).ToList(),
            Evidences = c.Evidences.Select(e => e.Description).ToList(),
            CaseReports = c.CaseReports.Select(r => r.ReportTitle).ToList()
        };
    }
}
