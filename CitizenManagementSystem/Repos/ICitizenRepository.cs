using CitizenManagementSystem.Models;

namespace CitizenManagementSystem.Repos
{
    public interface ICitizenRepository
    {
        Task<Citizen> AddAsync(Citizen citizen);
        Task DeleteAsync(int citizenId);
        Task<List<Citizen>> GetCitizensForAlertsAsync(string? city);
        Task<Citizen> UpdateAsync(Citizen citizen);
    }
}