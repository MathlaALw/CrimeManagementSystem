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

        
        //get  all participants of a case
        public async Task<List<ParticipantInCaseDto>> GetByCaseAsync(int caseId)
        {
            var query = _db.CaseParticipants
                .Include(cp => cp.Participant)
                .Where(cp => cp.CaseId == caseId);

            return await query
                .Select(cp => new ParticipantInCaseDto
                {
                    ParticipantId = cp.ParticipantId,
                    FullName = cp.Participant.FullName,
                    Phone = cp.Participant.Phone,
                    Notes = cp.Participant.Notes,
                    Role = cp.Role,
                    AddedAt = cp.AddedAt,
                    AddedByUserId = cp.AddedByUserId
                })
                .ToListAsync();
        }

        // get participants of a case by role
        public async Task<List<ParticipantInCaseDto>> GetByRoleAsync(int caseId, ParticipantRole role)
        {
            var query = _db.CaseParticipants
                .Include(cp => cp.Participant)
                .Where(cp => cp.CaseId == caseId && cp.Role == role);

            return await query
                .Select(cp => new ParticipantInCaseDto
                {
                    ParticipantId = cp.ParticipantId,
                    FullName = cp.Participant.FullName,
                    Phone = cp.Participant.Phone,
                    Notes = cp.Participant.Notes,
                    Role = cp.Role,
                    AddedAt = cp.AddedAt,
                    AddedByUserId = cp.AddedByUserId
                })
                .ToListAsync();
        }


        // Update participant details
        public async Task<bool> UpdateParticipantInCaseAsync(int participantId, UpdateParticipantDto dto)
        {
            // Find the existing participant by ID
            var participant = await _db.Participants.FindAsync(participantId);

            if (participant == null)
                return false; // Participant not found


            participant.FullName = dto.FullName;
            participant.Phone = dto.Phone;
            participant.Notes = dto.Notes;



            // Save changes
            await _db.SaveChangesAsync();

            return true;
        }

        // Delete participant 
        public async Task<bool> DeleteParticipantAsync(int participantId)
        {
            var participant = await _db.Participants.FindAsync(participantId);
            if (participant == null)
                return false;
            // Remove associated CaseParticipants entries
            var caseParticipants = _db.CaseParticipants.Where(cp => cp.ParticipantId == participantId);
            _db.CaseParticipants.RemoveRange(caseParticipants);
            // Remove the participant
            _db.Participants.Remove(participant);
            await _db.SaveChangesAsync();
            return true;


        }
    }
}
