using CitizenManagementSystem.DTOs;
using CitizenManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace CitizenManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitizenController : ControllerBase
    {
        private readonly ICitizenService _service;

        public CitizenController(ICitizenService service)
        {
            _service = service;
        }

        // Public: register a citizen
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateCitizenDto dto)
        {
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _service.RegisterCitizenAsync(dto);
            return Ok(new { id, message = "Citizen registered for alerts" });
        }

        // Internal use: get list of emails (for CrimeManagementSystem)
        [HttpPost("emails")]
        public async Task<IActionResult> GetEmails([FromBody] CitizenEmailFilterDto filter)
        {
            var emails = await _service.GetCitizenEmailsAsync(filter);
            return Ok(emails); // List<string>
        }

        // Internal use: update citizen info
        [HttpPut("Update")]
        public async Task<IActionResult> Update(int citizenId, [FromBody] UpdateCitizenDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _service.UpdateCitizenAsync(citizenId, dto);
            return Ok(new { message = "Citizen info updated" });
        }

        // Internal use: delete citizen
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int citizenId)
        {
            await _service.DeleteCitizenAsync(citizenId);
            return Ok(new { message = "Citizen deleted" });
        }
    }
}
