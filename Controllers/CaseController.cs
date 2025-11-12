using System.Security.Claims;
using Crime_Management_System.Attributes;
using Crime_Management_System.DTOs;
using Crime_Management_System.Models;
using Crime_Management_System.Services.Interfaces;
using Crime_Management_System.Servises;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Crime_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("fixed")]
    [AuthorizeRoles("Admin", "Investigator")]
    public class CasesController : ControllerBase
    {
        private readonly ICaseService _caseService;
        private readonly ICrimeReportService _crimeReportService;
        private readonly INotificationService _notifications;
        public CasesController(ICaseService caseService, ICrimeReportService crimeReportService, INotificationService notifications)
        {
            _caseService = caseService;
            _crimeReportService = crimeReportService;
            _notifications = notifications;
        }


        // Get current user id from JWT token
        private int CurrentUserId
        {
            get
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                    throw new UnauthorizedAccessException("User is not authenticated.");
                return int.Parse(userIdClaim);
            }
        }

        // GET: api/cases
        [HttpGet]
        [AuthorizeRoles("Admin", "Investigator", "Officer")]
        public async Task<IActionResult> GetAllCases()
        {
            var cases = await _caseService.GetAllCasesAsync();
            return Ok(cases);
        }

        // GET: api/cases/5
        [HttpGet("GetByID")]
        [AuthorizeRoles("Admin", "Investigator", "Officer")]
        public async Task<IActionResult> GetCase(int id)
        {
            try
            {

                if (id <= 0)
                {
                    return BadRequest(new { error = "Invalid case ID. ID must be greater than zero." });
                }

                var caseItem = await _caseService.GetCaseByIdAsync(id);


                if (caseItem == null)
                {
                    return NotFound(new { error = $"Case with ID {id} was not found." });
                }
                return Ok(caseItem);
            }
            catch (ArgumentException ex)
            {

                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {

                return Conflict(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred while retrieving the case.", details = ex.Message });
            }
        }

        //POST: api/cases
        [HttpPost]
        [AuthorizeRoles("Admin", "Investigator")]
        [ClearanceLevel("medium")]
        public async Task<IActionResult> CreateCase([FromBody] CreateCaseDto dto)
        {
            if (dto == null)
                return BadRequest("Case data is required.");


            // check if the data is valid
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _caseService.CreateCaseAsync(dto, CurrentUserId);


            // If creation failed and the result is null 
            if (result is null)
                return BadRequest(new
                {
                    message = "Invalid data: case number may already exist or crime report IDs are incorrect."
                });
            // Successful creation

            var (id, message) = result.Value;
            return CreatedAtAction(nameof(GetCase), new { id = id }, new { id = id, message = message });
        }

        // PUT: api/cases/5
        [HttpPut("UpdateByID")]
        [AuthorizeRoles("Admin", "Investigator", "Officer")]
        public async Task<IActionResult> UpdateCase(int id, [FromBody] UpdateCaseDto caseItem)
        {
            if (caseItem == null)
                return BadRequest(new { error = "Case data is required." });

            try
            {
                var existingCase = await _caseService.GetCaseByIdAsync(id);
                if (existingCase == null)
                    return NotFound(new { error = $"Case with ID {id} not found." });

                var updatedCase = await _caseService.UpdateCaseAsync(id, caseItem);

                await _notifications.SendCaseUpdateNotificationAsync(updatedCase);

                return Ok(new
                {
                    message = "Case updated successfully.",
                    updatedCase
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Unexpected error: {ex.Message}" });
            }
        }


        // DELETE: api/cases/5
        [HttpDelete("deleteCase")]
        [AuthorizeRoles("Admin")]
        public async Task<IActionResult> DeleteCase(int id)
        {
            try
            {
                var caseItem = await _caseService.GetCaseByIdAsync(id);
                if (caseItem == null)
                    return NotFound(new { message = "Case not found." });

                await _caseService.DeleteCaseAsync(id);
                return Ok(new { message = "Case deleted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An unexpected error occurred while deleting the case." });
            }
        }

        // get all crime report 
        // GET: api/cases/public/report
        [HttpGet("public/report")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCrimeReports()
        {
            var reports = await _crimeReportService.GetAllReportsAsync();

            if (reports == null || !reports.Any())
                return NotFound(new { message = "No crime reports found." });

            return Ok(reports);
        }
        [HttpGet("details")]
        [AuthorizeRoles("Admin", "Investigator", "Officer")]
        public async Task<IActionResult> GetCaseDetails(int id)
        {
            var details = await _caseService.GetCaseDetailsAsync(id);
            if (details == null)
                return NotFound(new { message = "Case not found." });
            return Ok(details);
        }


    }
}

