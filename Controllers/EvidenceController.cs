using Crime_Management_System.DTOs;
using Crime_Management_System.Models;
using Crime_Management_System.Servises;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AutoMapper;

namespace Crime_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(AuthenticationSchemes = "Basic", Policy = "OfficerOrHigher")]
    public class EvidenceController : ControllerBase
    {
        private readonly IEvidenceService _service;
        private readonly IWebHostEnvironment _env; // To get the root path for file storage

        public EvidenceController(IEvidenceService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }


        // Get current user id from claims -> track who is making the changes
        // private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        private int CurrentUserId
        {
            get
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    // This should not happen because of the Authorize attribute, but just in case.
                    throw new UnauthorizedAccessException("User is not authenticated.");
                }
                return int.Parse(userIdClaim);
            }
        }
        // Create text evidence
        [HttpPost("Create text evidence")]
        public async Task<IActionResult> CreateText(CreateTextEvidenceDto dto)
        {
            if (dto is null) return BadRequest("Payload is required.");
            var res = await _service.CreateTextAsync(dto, CurrentUserId);
            return res is null ?
                NotFound("Case not found") :
                Ok(new { res?.id, res?.message });
        }

        // Create image evidence
        [HttpPost("Create image evidence")]
        public async Task<IActionResult> CreateImage([FromForm] CreateImageEvidenceDto dto)
        {
            if (dto is null) return BadRequest("Payload is required.");
            var res = await _service.CreateImageAsync(dto, CurrentUserId, _env.ContentRootPath);
            return res is null ?
                BadRequest("Invalid case or image") :
                Ok(new { res?.id, res?.message });
        }

        // Get evidence by id
        [HttpGet("Get Evidence by id ")]
        public async Task<IActionResult> Get(int id)
        {
            var e = await _service.GetAsync(id);
            if (e == null || e.IsSoftDeleted) return NotFound();
            return Ok(new
            {
                e.Id,
                e.CaseId,
                e.Type,
                e.TextContent,
                e.FileUrl,
                e.MimeType,
                e.SizeBytes,
                e.Remarks,
                e.CreatedAt
            });
        }

        // Get image evidence file by Id
        [HttpGet("Get image evidence file by Id")]
        public async Task<IActionResult> Image(int id)
        {
            var res = await _service.GetImageAsync(id, _env.ContentRootPath);
            if (res is null)
                return BadRequest("Evidence is not an image or missing");
            // MemoryStream to match the correct File(...) overload
            var stream = new MemoryStream(res.Value.bytes);
            // If you have a filename, pass it as the 3rd argument; otherwise omit
            return File(stream, res.Value.mime, enableRangeProcessing: true);
        }

        // Update evidence by id
        [HttpPut("Evidence update by  {id:int}")]
        public async Task<IActionResult> Update(int id, UpdateEvidenceDto dto)
        {
            var ok = await _service.UpdateAsync(id, dto, CurrentUserId);
            return ok ?
                Ok(new { message = "Evidence updated" }) :
                BadRequest("Invalid request");


        }

        // Soft delete evidence by id
        [HttpDelete("Soft delete evidence by {id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var ok = await _service.SoftDeleteAsync(id, CurrentUserId);
            return ok ?
                Ok(new { message = "Evidence soft-deleted" }) :
                NotFound();
        }


        


    }
}