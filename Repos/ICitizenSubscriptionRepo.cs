using Crime_Management_System.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crime_Management_System.Repos
{
    public interface ICitizenSubscriptionRepo : IGenericRepo<CitizenSubscription>
    {
        Task<List<CitizenSubscription>> GetActiveByCityAsync(string city);
        Task<CitizenSubscription?> GetByEmailAsync(string email);
    }
}
