using Crime_Management_System.DTOs;
using Crime_Management_System.Models;
using Crime_Management_System.Repos;

namespace Crime_Management_System.Servises
{
    public interface ICitizenSubscriptionService 
    {
        Task<CitizenSubscription> CreateAsync(CreateCitizenSubscriptionDto dto);
        Task<bool> UnsubscribeAsync(string email);

        Task<List<CitizenSubscription>> GetSubscribersForNewCrimesAsync(string city);
        Task<List<CitizenSubscription>> GetSubscribersForCaseUpdatesAsync(string city);
        Task<List<CitizenSubscription>> GetSubscribersForAlertsAsync(string city);
    }
}
