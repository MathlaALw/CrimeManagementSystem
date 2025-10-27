using Crime_Management_System.DTOs;
using Crime_Management_System.Models;
using Crime_Management_System.Servises;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Crime_Management_System.Controllers
{
    [ApiController]
    [Route("api/evidence")]
    //[Authorize(AuthenticationSchemes = "Basic", Policy = "OfficerOrHigher")]
    public class EvidenceController : ControllerBase
    {
        private readonly IEvidenceService _service;
        private readonly IWebHostEnvironment _env;

        public EvidenceController(IEvidenceService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        [HttpPost("text")]
        public async Task<IActionResult> CreateText(CreateTextEvidenceDto dto)
        {
            var res = await _service.CreateTextAsync(dto, CurrentUserId);
            return res is null ?
                NotFound("Case not found") :
                Ok(new { res?.id, res?.message });
        }

        [HttpPost("image")]
        public async Task<IActionResult> CreateImage([FromForm] CreateImageEvidenceDto dto)
        {
            var res = await _service.CreateImageAsync(dto, CurrentUserId, _env.ContentRootPath);
            return res is null ?
                BadRequest("Invalid case or image") :
                Ok(new { res?.id, res?.message });
        }

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

        [HttpGet("{id:int}/image")]
        public async Task<IActionResult> Image(int id)
        {
            var res = await _service.GetImageAsync(id, _env.ContentRootPath);
            return res is null ?
                BadRequest("Evidence is not an image or missing") :
                File(res.Value.bytes, res.Value.mime, enableRangeProcessing: true);
        }

        [HttpPut("Evidence update by  {id:int}")]
        public async Task<IActionResult> Update(int id, UpdateEvidenceDto dto)
        {
            var ok = await _service.UpdateAsync(id, dto, CurrentUserId);
            return ok ?
                Ok(new { message = "Evidence updated" }) :
                BadRequest("Invalid request");
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var ok = await _service.SoftDeleteAsync(id, CurrentUserId);
            return ok ?
                Ok(new { message = "Evidence soft-deleted" }) :
                NotFound();
        }

    }
}