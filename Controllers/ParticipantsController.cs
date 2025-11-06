using Crime_Management_System.DTOs;
using Crime_Management_System.Models;
using Crime_Management_System.Servises;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Crime_Management_System.Controllers
{
    [ApiController]
   // [Authorize]
    [Route("api/[controller]")]
    [Authorize(Policy = "InvestigatorOrAbove")]
    public class ParticipantsController : ControllerBase
    {
        private readonly IParticipantService _service;

        public ParticipantsController(IParticipantService service)
        {
            _service = service;
        }
        // Get current user id from JWT token
        private int? CurrentUserIdOrNull
        {
            get
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                    return null;

                return int.Parse(userIdClaim);
            }
        }

        // Create a new participant
        [HttpPost("CreateNewParticipant")]
        public async Task<IActionResult> Create([FromBody] AddParticipantDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (dto == null)
                return BadRequest("Participant data is required.");
            var participant = await _service.CreateAsync(dto);

            // Return basic info (you can expand if you want)
            return Ok(new
            {
                participant.Id,
                participant.FullName,
                participant.Phone,
                participant.Notes
            });
        }

        // Add participant to a case
        [HttpPost("add-to-case")]
        public async Task<IActionResult> AddToCase(int caseId, [FromBody] AddParticipantToCaseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var addedByUserId = CurrentUserIdOrNull;

            var success = await _service.AddToCaseAsync(caseId, dto, addedByUserId);

            if (!success)
                return BadRequest(new { message = "Invalid caseId or participantId, or case does not exist." });

            return Ok(new { message = "Participant added to case successfully." });
        }

        // Get all participants by caseId
        [HttpGet("allParticipants")]
        public async Task<IActionResult> GetAllParticipants(int caseId)
        {
            var list = await _service.GetByCaseAsync(caseId);

            if (list == null || !list.Any())
                return NotFound(new { message = "No participants found for this case." });

            return Ok(list);
        }
        // get allParticipants by role = Suspect by caseId
        [HttpGet("allSuspects")]
        public async Task<IActionResult> GetSuspects(int caseId)
        {
            var list = await _service.GetByRoleAsync(caseId, ParticipantRole.Suspect);

            if (list == null || !list.Any())
                return NotFound(new { message = "No suspects found for this case." });

            return Ok(list);
        }
        // get allParticipants by role = Victim by caseId
        [HttpGet("allVictims")]
        public async Task<IActionResult> GetVictims(int caseId)
        {
            var list = await _service.GetByRoleAsync(caseId, ParticipantRole.Victim);

            if (list == null || !list.Any())
                return NotFound(new { message = "No victims found for this case." });

            return Ok(list);
        }

        // get allParticipants by role = Witness by caseId
        [HttpGet("allWitnesses")]
        public async Task<IActionResult> GetWitnesses(int caseId)
        {
            var list = await _service.GetByRoleAsync(caseId, ParticipantRole.Witness);

            if (list == null || !list.Any())
                return NotFound(new { message = "No witnesses found for this case." });

            return Ok(list);
        }


        [HttpPut("UpdateParticipant")]
        [Authorize(Policy = "InvestigatorOrAbove")]
        public async Task<IActionResult> UpdateParticipant(int participantId, [FromBody] UpdateParticipantDto dto)
        {
            var success = await _service.UpdateParticipantInCaseAsync(participantId, dto);

            if (!success)
                return NotFound(new { message = "Participant not found" });

            return Ok(new { message = "Participant updated successfully" });
        }

        [HttpDelete("DeleteParticipantById")]
        [Authorize(Policy = "InvestigatorOrAbove")]
        public async Task<IActionResult> DeleteParticipant(int participantId)
        {
            var success = await _service.DeleteParticipantAsync(participantId);

            if (!success)
                return NotFound(new { message = "Participant not found" });

            return Ok(new { message = "Participant deleted successfully" });
        }

    }
}
