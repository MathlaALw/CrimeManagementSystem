using Crime_Management_System.DTOs;
using Crime_Management_System.Models;
using Crime_Management_System.Repos;



namespace Crime_Management_System.Servises
{

    public class CitizenSubscriptionService : ICitizenSubscriptionService
    {
        private readonly ICitizenSubscriptionRepo _repo;

        public CitizenSubscriptionService(ICitizenSubscriptionRepo repo)
        {
            _repo = repo;
        }

        public async Task<CitizenSubscription> CreateAsync(CreateCitizenSubscriptionDto dto)
        {
            var existing = await _repo.GetByEmailAsync(dto.Email);

            if (existing != null && existing.IsActive)
                throw new InvalidOperationException("This email is already subscribed.");

            if (existing != null)
            {
                existing.FullName = dto.FullName;
                existing.City = dto.City;
                existing.ReceiveNewCrimes = dto.ReceiveNewCrimes;
                existing.ReceiveCaseUpdates = dto.ReceiveCaseUpdates;
                existing.ReceiveAlerts = dto.ReceiveAlerts;
                existing.IsActive = true;
                existing.SubscribedAtUtc = DateTime.UtcNow;

                _repo.Update(existing);
            }
            else
            {
                var entity = new CitizenSubscription
                {
                    FullName = dto.FullName,
                    Email = dto.Email,
                    City = dto.City,
                    ReceiveNewCrimes = dto.ReceiveNewCrimes,
                    ReceiveCaseUpdates = dto.ReceiveCaseUpdates,
                    ReceiveAlerts = dto.ReceiveAlerts,
                    IsActive = true,
                    SubscribedAtUtc = DateTime.UtcNow
                };

                await _repo.AddAsync(entity);
            }

            await _repo.SaveAsync(); 

            // Return latest version by email
            return (await _repo.GetByEmailAsync(dto.Email))!;
        }

        public async Task<bool> UnsubscribeAsync(string email)
        {
            var existing = await _repo.GetByEmailAsync(email);
            if (existing == null) return false;

            existing.IsActive = false;
            _repo.Update(existing);
            await _repo.SaveAsync();

            return true;
        }

        public async Task<List<CitizenSubscription>> GetSubscribersForNewCrimesAsync(string city)
        {
            var list = await _repo.GetActiveByCityAsync(city);
            return list.Where(x => x.ReceiveNewCrimes).ToList();
        }

        public async Task<List<CitizenSubscription>> GetSubscribersForCaseUpdatesAsync(string city)
        {
            var list = await _repo.GetActiveByCityAsync(city);
            return list.Where(x => x.ReceiveCaseUpdates).ToList();
        }

        public async Task<List<CitizenSubscription>> GetSubscribersForAlertsAsync(string city)
        {
            var list = await _repo.GetActiveByCityAsync(city);
            return list.Where(x => x.ReceiveAlerts).ToList();
        }
    }
}
