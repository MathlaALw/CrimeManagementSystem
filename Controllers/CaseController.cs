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

        // GET: api/cases
        [HttpGet]
        [AuthorizeRoles("Admin", "Investigator", "Officer")] 
        public async Task<IActionResult> GetCases()
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

        // POST: api/cases
        [HttpPost]
        [AuthorizeRoles("Admin", "Investigator")]
        [ClearanceLevel("medium")] 
        public async Task<IActionResult> CreateCase([FromBody] CreateCaseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newCase = new Case
            {
                CaseNumber = dto.CaseNumber,
                Name = dto.Name,
                Description = dto.Description,
                AreaCity = dto.AreaCity,
                CaseType = dto.CaseType,
                AuthorizationLevel = dto.AuthorizationLevel,
                Status = dto.Status,
                CreatedByUserId = dto.CreatedByUserId,
                CaseReports = new List<CaseReport>()
            };
            if (dto.CrimeReportIds != null && dto.CrimeReportIds.Any())
            {
                newCase.CaseReports = dto.CrimeReportIds
                 .Select(reportId => new CaseReport { ReportId = reportId, Case = newCase })
                 .ToList();

            }

            var created = await _caseService.CreateCaseAsync(newCase);

            return CreatedAtAction(nameof(GetCase), new { id = created.Id }, created);
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
        [HttpDelete("{id}")]
        [AuthorizeRoles("Admin")]
        [ClearanceLevel("high")] 
        public async Task<IActionResult> DeleteCase(int id)
        {
            var caseItem = await _caseService.GetCaseByIdAsync(id);
            if (caseItem == null)
                return NotFound();

            await _caseService.DeleteCaseAsync(id);
            return NoContent();
        }

        // GET: api/cases/public/report
        [HttpGet("public/report")]
        [AllowAnonymous] 
        public IActionResult ReportCrime()
        {
           
            return Ok("Crime report endpoint (public).");
        }
    }
}
