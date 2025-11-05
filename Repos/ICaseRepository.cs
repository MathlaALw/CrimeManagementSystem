using Crime_Management_System.Models;
using System.Collections.Generic;

namespace Crime_Management_System.Repositories.Implementations
{
    public interface ICaseRepository
    {
        Task<IEnumerable<Case>> GetCasesWithDetailsAsync();
        Task<Case?> GetCaseWithDetailsByIdAsync(int id);
         Task<IEnumerable<Case>> GetAllAsync();

    }
}