using Crime_Management_System.Attributes;
using Crime_Management_System.DTOs;
using Crime_Management_System.Servises;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Crime_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "InvestigatorOrAbove")]
    public class CaseAssigneesController : ControllerBase
    {
        private readonly ICaseAssigneeService _service;

        public CaseAssigneesController(ICaseAssigneeService service)
        {
            _service = service;
        }

        // POST: api/caseassignees/assign-officer
        [HttpPost("assign-officer")]
        [Authorize(Policy = "InvestigatorOrAbove")]

        public async Task<IActionResult> AssignOfficer([FromBody] AssignOfficerDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message) = await _service.AssignOfficerAsync(dto);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
     
        
        }

        // get all assignees of a case
        
        [HttpGet("case/{caseId:int}")]
        public async Task<IActionResult> GetAssigneesByCase(int caseId)
        {
            var assignees = await _service.GetByCaseIdAsync(caseId);

            if (assignees == null || !assignees.Any())
                return NotFound(new { message = "No assignees found for this case." });

            // Simple projection
            var result = assignees.Select(a => new
            {
                a.Id,
                a.CaseId,
                a.UserId,
                UserFullName = a.User?.FullName,
                a.AssignedRole,
                a.ProgressStatus,
                a.AssignedAt
            });

            return Ok(result);
        }
    }
}
