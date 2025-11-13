using CitizenManagementSystem.DTOs;
using CitizenManagementSystem.Models;
using CitizenManagementSystem.Repos;

namespace CitizenManagementSystem.Services
{
    public class CitizenService : ICitizenService
    {
        private readonly ICitizenRepository _repo;

        public CitizenService(ICitizenRepository repo)
        {
            _repo = repo;
        }
        // Register new citizen
        public async Task<int> RegisterCitizenAsync(CreateCitizenDto dto)
        {
            var entity = new Citizen
            {
                FullName = dto.FullName,
                Email = dto.Email,
                City = dto.City,
                DateOfBirth = dto.DateOfBirth


            };

            var created = await _repo.AddAsync(entity);
            return created.Id;
        }
        // Get citizen emails for alerts
        public async Task<List<string>> GetCitizenEmailsAsync(CitizenEmailFilterDto filter)
        {
            var citizens = await _repo.GetCitizensForAlertsAsync(filter.City);

            return citizens
                .Select(c => c.Email)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        // Update citizen info
        public async Task UpdateCitizenAsync(int citizenId, UpdateCitizenDto dto)
        {
            var citizen = new Citizen
            {
                Id = citizenId,
                FullName = dto.FullName,
                Email = dto.Email,
                City = dto.City,

            };
            await _repo.UpdateAsync(citizen);
        }

        // Delete citizen
        public async Task DeleteCitizenAsync(int citizenId)
        {
            await _repo.DeleteAsync(citizenId);
        }
    }
}
