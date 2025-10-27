using Crime_Management_System.Data;
using Crime_Management_System.DTOs;
using Crime_Management_System.Models;
using Crime_Management_System.Repos;
using Microsoft.EntityFrameworkCore;

namespace Crime_Management_System.Servises
{
    public class ReportService : IReportService
    {
        private readonly IReportRepo _repo;
        private readonly CrimeDbContext _db;
        public ReportService(IReportRepo repo, CrimeDbContext db) { _repo = repo; _db = db; }

        // Submit a crime report
        public async Task<(int reportId, string message)?> SubmitAsync(SubmitCrimeReportDto dto)
        {
            // Validate reported_by: null (citizen) OR Admin/Investigator
            if (dto.ReportedByUserId.HasValue)
            {
                var u = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == dto.ReportedByUserId);
                if (u == null || (u.Role != UserRole.Admin && u.Role != UserRole.Investigator))
                    return null;
            }

            var r = new CrimeReport
            {
                Title = dto.Title,
                Description = dto.Description,
                AreaCity = dto.AreaCity,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                ReportedByUserId = dto.ReportedByUserId
            };

            await _repo.AddAsync(r);
            await _repo.SaveAsync();
            return (r.Id, "Report submitted. Use reportId to track status.");
        }

        // Get report status by ID
        public async Task<(int id, string status)?> GetStatusAsync(int id)
        {
            var r = await _repo.GetReadOnlyAsync(id);
            return r == null ? null : (r.Id, r.Status);
        }
    }
}
