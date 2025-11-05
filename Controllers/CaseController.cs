using System.Security.Claims;
using Crime_Management_System.Attributes;
using Crime_Management_System.DTOs;
using Crime_Management_System.Models;
using Crime_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Crime_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRoles("Admin", "Investigator")] 
    public class CasesController : ControllerBase
    {
        private readonly ICaseService _caseService;

        public CasesController(ICaseService caseService)
        {
            _caseService = caseService;
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
        [HttpGet("{id}")]
        [AuthorizeRoles("Admin", "Investigator", "Officer")]
        public async Task<IActionResult> GetCase(int id)
        {
            var caseItem = await _caseService.GetCaseByIdAsync(id);
            if (caseItem == null)
                return NotFound();

            return Ok(caseItem);
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
        [HttpPut("{id}")]
        [AuthorizeRoles("Admin", "Investigator")]
        [ClearanceLevel("medium")]
        public async Task<IActionResult> UpdateCase(int id, [FromBody] Case caseItem)

        {
            if (id != caseItem.Id)
                return BadRequest("Case ID mismatch");

            var existingCase = await _caseService.GetCaseByIdAsync(id);
            if (existingCase == null)
                return NotFound();

            await _caseService.UpdateCaseAsync(caseItem);
            return NoContent();
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
        public IActionResult ReportCrime()
        {
           
           
            return Ok("Crime report endpoint (public).");
        }
    }
}
