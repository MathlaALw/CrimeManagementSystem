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
    }
}
