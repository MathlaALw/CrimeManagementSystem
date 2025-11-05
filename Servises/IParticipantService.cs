using Crime_Management_System.DTOs;
using Crime_Management_System.Models;

namespace Crime_Management_System.Servises
{
    public interface IParticipantService
    {
        Task<bool> AddToCaseAsync(int caseId, AddParticipantToCaseDto dto, int? addedByUserId);
        Task<Participant> CreateAsync(AddParticipantDto dto);

        Task<List<ParticipantInCaseDto>> GetByCaseAsync(int caseId);

        Task<List<ParticipantInCaseDto>> GetByRoleAsync(int caseId, ParticipantRole role);

        Task<bool> UpdateParticipantInCaseAsync(int participantId,UpdateParticipantDto dto);

        Task<bool> DeleteParticipantAsync(int participantId);
    }
}