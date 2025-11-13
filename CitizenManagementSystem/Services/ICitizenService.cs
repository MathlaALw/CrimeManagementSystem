using CitizenManagementSystem.DTOs;

namespace CitizenManagementSystem.Services
{
    public interface ICitizenService
    {
        Task DeleteCitizenAsync(int citizenId);
        Task<List<string>> GetCitizenEmailsAsync(CitizenEmailFilterDto filter);
        Task<int> RegisterCitizenAsync(CreateCitizenDto dto);
        Task UpdateCitizenAsync(int citizenId, UpdateCitizenDto dto);
    }
}