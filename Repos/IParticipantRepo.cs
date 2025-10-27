
using Crime_Management_System.Models;

namespace Crime_Management_System.Repos
{
    public interface IParticipantRepo
    {
        Task AddAsync(Participant p);
        Task<bool> ExistsAsync(int id);
        Task SaveAsync();
    }
}