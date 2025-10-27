using Crime_Management_System.Data;
using Crime_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Crime_Management_System.Repos
{
    public class ParticipantRepo : GenericRepo<Participant>, IParticipantRepo
    {
        public ParticipantRepo(CrimeDbContext db) : base(db) { }

        public async Task<bool> ExistsAsync(int id) => await _table.AnyAsync(p => p.Id == id);
    }
}
