using Crime_Management_System.Data;
using Crime_Management_System.DTOs;
using Crime_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Crime_Management_System.Servises
{
    public class CaseAssigneeService : ICaseAssigneeService
    {
        private readonly CrimeDbContext _db;

        public CaseAssigneeService(CrimeDbContext db)
        {
            _db = db;
        }

        // Assign Officer Async
        public async Task<(bool success, string message)> AssignOfficerAsync(AssignOfficerDto dto)
        {

            // Load the case
            var caseEntity = await _db.Cases.FirstOrDefaultAsync(c => c.Id == dto.CaseId);
            if (caseEntity == null)
            {
                return (false, "Case not found");
            }

            // Load the officer
            var officer = await _db.Users.FirstOrDefaultAsync(u => u.Id == dto.OfficerId && u.Role == UserRole.Officer);

            if (officer == null)
            {
                return (false, "Officer not found");
            }

            // // officer's clearance must be >= case's authorization level
            if (officer.ClearanceLevel < caseEntity.AuthorizationLevel)
            {
                return (false, $"Officer clearance ({officer.ClearanceLevel}) is lower than case authorization level ({caseEntity.AuthorizationLevel}).");
            }

            // Check if already assigned
            var alreadyAssigned = await _db.CaseAssignees.AnyAsync(a =>
               a.CaseId == dto.CaseId && a.UserId == dto.OfficerId);

            if (alreadyAssigned)
                return (false, "Officer is already assigned to this case.");
            
           
            // Create new assignment
            var assignee = new CaseAssignee
            {
                CaseId = dto.CaseId,
                UserId = dto.OfficerId,
                AssignedRole = "Officer",
                ProgressStatus = "pending",
                AssignedAt = DateTime.UtcNow
            };

            _db.CaseAssignees.Add(assignee);
            await _db.SaveChangesAsync();

            return (true, "Officer assigned successfully.");

        }


    }
}
