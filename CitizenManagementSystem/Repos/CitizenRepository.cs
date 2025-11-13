
using CitizenManagementSystem.Data;
using CitizenManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace CitizenManagementSystem.Repos
{
    public class CitizenRepository : ICitizenRepository
    {
        private readonly CitizenDbContext _db;

        public CitizenRepository(CitizenDbContext db)
        {
            _db = db;
        }
        // add new citizen

        public async Task<Citizen> AddAsync(Citizen citizen)
        {
            _db.Citizens.Add(citizen);
            await _db.SaveChangesAsync();
            return citizen;
        }
        // get citizens for alerts
        public async Task<List<Citizen>> GetCitizensForAlertsAsync(
            string? city)
        {
            var query = _db.Citizens.AsQueryable();

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(c => c.City == city);


            return await query.ToListAsync();
        }

        //update citizen
        public async Task<Citizen> UpdateAsync(Citizen citizen)
        {
            _db.Citizens.Update(citizen);
            await _db.SaveChangesAsync();
            return citizen;
        }

        // delete citizen
        public async Task DeleteAsync(int citizenId)
        {
            var citizen = await _db.Citizens.FindAsync(citizenId);
            if (citizen != null)
            {
                _db.Citizens.Remove(citizen);
                await _db.SaveChangesAsync();
            }
        }
    }
}
