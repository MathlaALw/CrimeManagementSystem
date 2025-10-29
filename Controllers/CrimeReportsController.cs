using Crime_Management_System.Data;
using Crime_Management_System.DTOs;
using Crime_Management_System.Models;
using Crime_Management_System.Services;
using Crime_Management_System.Servises;
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
        private readonly object _CrimeReportService;
        private object? crimeReportService;

        public CrimeReportsController(CrimeDbContext context, ICrimeReportService crimeReportService)
        {
            _context = context;
            _CrimeReportService = crimeReportService;
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
      //  [HttpPost]
        //public async Task<IActionResult> SubmitCrimeReport([FromBody] CrimeReport report)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var createdReport = await _crimeReportService.CreateReportAsync(report);
        //    return Ok(new { reportId = createdReport.Id });
        //}

        //// GET: api/CrimeReport/{id}/status
        //[HttpGet("{id}/status")]
        //public async Task<IActionResult> GetReportStatus(int id)
        //{
        //    var report = await _crimeReportService.GetReportByIdAsync(id);
        //    if (report == null)
        //        return NotFound();

        //    return Ok(new { status = report.Status });
        //}
    
    }
}
