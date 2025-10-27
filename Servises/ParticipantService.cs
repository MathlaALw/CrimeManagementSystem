using Crime_Management_System.Data;
using Crime_Management_System.DTOs;
using Crime_Management_System.Models;
using Crime_Management_System.Repos;
using Microsoft.EntityFrameworkCore;

namespace Crime_Management_System.Servises
{
    public class ParticipantService : IParticipantService
    {
        private readonly IParticipantRepo _repo;
        private readonly CrimeDbContext _db;

        public ParticipantService(IParticipantRepo repo, CrimeDbContext db)
        {
            _repo = repo;
            _db = db;
        }

        public async Task<Participant> CreateAsync(AddParticipantDto dto)
        {
            var p = new Participant
            {
                FullName = dto.FullName,
                Phone = dto.Phone,
                Notes = dto.Notes
            };

            await _repo.AddAsync(p);
            await _repo.SaveAsync();
            return p;
        }

        public async Task<bool> AddToCaseAsync(int caseId, AddParticipantToCaseDto dto, int? addedByUserId)
        {
            var c = await _db.Cases.AsNoTracking().FirstOrDefaultAsync(x => x.Id == caseId);
            if (c == null) return false;

            if (!await _repo.ExistsAsync(dto.ParticipantId)) return false;

            _db.CaseParticipants.Add(new CaseParticipant
            {
                CaseId = caseId,
                ParticipantId = dto.ParticipantId,
                Role = dto.Role,
                AddedByUserId = addedByUserId,
                AddedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
