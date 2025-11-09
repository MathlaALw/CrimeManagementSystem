using Crime_Management_System.Data;
using Crime_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Crime_Management_System.Repos
{
    public class CitizenSubscriptionRepo : GenericRepo<CitizenSubscription>, ICitizenSubscriptionRepo
    {
        // injecting the DbContext to the base GenericRepo
        public CitizenSubscriptionRepo(CrimeDbContext context) : base(context) { }

        // Get active subscriptions by city
        public async Task<List<CitizenSubscription>> GetActiveByCityAsync(string city)
        {
            return await _table
                .Where(cs => cs.IsActive && cs.City.ToLower() == city.ToLower())
                .ToListAsync();
        }
        // Get subscription by email
        public async Task<CitizenSubscription?> GetByEmailAsync(string email)
        {
            return await _table
                .FirstOrDefaultAsync(cs => cs.Email.ToLower() == email.ToLower());
        }
    }
}

