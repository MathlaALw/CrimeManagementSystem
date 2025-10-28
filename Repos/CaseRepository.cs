using Crime_Management_System.Models;
using Microsoft.EntityFrameworkCore;
using Crime_Management_System.Repos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crime_Management_System.Data;

namespace Crime_Management_System.Repositories.Implementations
{
    public class CaseRepository : GenericRepo<Case>, ICaseRepository
    {
        public CaseRepository(CrimeDbContext context) : base(context)
        {
        }

        public async Task<Case?> GetByCaseNumberAsync(string caseNumber)
        {
            return await _table
                .Include(c => c.CreatedByUser)
                .FirstOrDefaultAsync(c => c.CaseNumber == caseNumber);
        }

        public async Task<IEnumerable<Case>> GetCasesByUserAsync(int userId)
        {
            return await _table
                .Include(c => c.CreatedByUser)
                .Where(c => c.CreatedByUserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Case>> GetAssignedCasesAsync(int officerId)
        {
            return await _table
                .Include(c => c.CaseAssignees)
                .ThenInclude(ca => ca.User)
                .Where(c => c.CaseAssignees.Any(a => a.UserId == officerId))
                .ToListAsync();
        }

        public async Task<bool> CaseNumberExistsAsync(string caseNumber)
        {
            return await _table.AnyAsync(c => c.CaseNumber == caseNumber);
        }

        public async Task<Case> CreateAsync(Case caseEntity)
        {
            await _table.AddAsync(caseEntity);
            await _context.SaveChangesAsync();
            return caseEntity;
        }

        public async Task<Case> UpdateAsync(Case caseEntity)
        {
            var existingCase = await _table
                .Include(c => c.CaseAssignees)
                .Include(c => c.CaseParticipants)
                .Include(c => c.Evidences)
                .Include(c => c.CaseReports)
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

            await _context.SaveChangesAsync();
            return existingCase;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var caseEntity = await _table.FindAsync(id);
            if (caseEntity == null)
                return false;

            _table.Remove(caseEntity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
