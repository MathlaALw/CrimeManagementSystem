using Crime_Management_System.Data;
using Crime_Management_System.DTOs;
using Crime_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Crime_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class CrimeReportsController : ControllerBase
    {
        private readonly CrimeDbContext _context;

        public CrimeReportsController(CrimeDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> ReportCrime([FromBody] CrimeReportCreateDto dto)
        {
            var report = new CrimeReport
            {
                Description = dto.Description,
                AreaCity = dto.AreaCity,
                
                ReportDateTime = DateTime.UtcNow,
                Status = "Pending",
                CaseReports = new List<CaseReport>()
            };
            

            _context.CrimeReports.Add(report);
            await _context.SaveChangesAsync();

            return Ok(new { ReportId = report.Id });
        }

        [HttpGet("{reportId}")]
        public IActionResult GetReportStatus(int reportId)
        {
            var report = _context.CrimeReports
                .FirstOrDefault(r => r.Id == reportId);

            if (report == null) return NotFound();

            return Ok(new { report.Status, report.CaseReports });
        }
    }
}
