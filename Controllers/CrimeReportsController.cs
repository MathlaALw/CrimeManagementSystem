using Crime_Management_System.Data;
using Crime_Management_System.DTOs;
using Crime_Management_System.Models;

using Crime_Management_System.Servises;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Crime_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class CrimeReportsController : ControllerBase
    {
        private readonly CrimeDbContext _context;
        private readonly ICrimeReportService _CrimeReportService;
        private readonly INotificationService _notifications;

        public CrimeReportsController(CrimeDbContext context, ICrimeReportService crimeReportService, INotificationService notifications)
        {
            _context = context;
            _CrimeReportService = crimeReportService;
            _notifications = notifications;
        }


        [HttpPost]
        public async Task<IActionResult> ReportCrime([FromBody] CrimeReportCreateDto dto)
        {

            if (dto == null)
                return BadRequest(new { message = "Report data is required." });

            // Get the current user (if any) 
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //int? currentUserId = string.IsNullOrEmpty(userId) ? null : int.Parse(userId);

            int? currentUserId = null;
            string? role = null;

            if (User?.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var id))
                    currentUserId = id;

                role = User.FindFirstValue(ClaimTypes.Role); // e.g. "Admin", "Investigator", "Citizen"
            }

            // validate reportedby role if user exists
            if (currentUserId != null)
            {


                if (string.IsNullOrEmpty(role) || (role != "Admin" && role != "Investigator" && role != "Citizen"))
                {
                    return BadRequest(new { message = "Only Citizens, Admins, or Investigators can file crime reports." });
                }



            }
            var report = new CrimeReport
            {
                Title = dto.Title,
                Description = dto.Description,
                AreaCity = dto.AreaCity,
                ReportDateTime = DateTime.UtcNow,
                Status = "Pending",
                ReportedByUserId = currentUserId,
                CaseReports = new List<CaseReport>()

            };
          
            _context.CrimeReports.Add(report);
            await _notifications.SendNewCrimeReportNotificationAsync(report);
            await _context.SaveChangesAsync();
           

            return Ok(new 
            {

                message = "Crime report submitted successfully.",
                ReportId = report.Id,
                Status = report.Status

            });
        }

        [HttpGet("GetReportStatus")]
        public IActionResult GetReportStatus(int reportId)

        {


            var report = _context.CrimeReports
                .FirstOrDefault(r => r.Id == reportId);

            if (report == null) return NotFound(new { message = "Report not found." });

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
