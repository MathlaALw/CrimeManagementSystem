using Crime_Management_System.DTOs;
using Crime_Management_System.Models;
using Crime_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Crime_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaseController : ControllerBase
    {
        private readonly ICaseService _caseService;

        public CaseController(ICaseService caseService)
        {
            _caseService = caseService;
        }

        // GET: api/case
        [HttpGet]
        public async Task<IActionResult> GetAllCases()
        {
            var cases = await _caseService.GetAllCasesAsync();
            return Ok(cases);
        }

        // GET: api/case/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCase(int id)
        {
            var caseItem = await _caseService.GetCaseByIdAsync(id);
            if (caseItem == null)
                return NotFound();

            return Ok(caseItem);
        }

        // POST: api/case
        [HttpPost]
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
                CreatedByUserId = dto.CreatedByUserId
            };

            var created = await _caseService.CreateCaseAsync(newCase);
            return CreatedAtAction(nameof(GetCase), new { id = created.Id }, created);
        }

        // PUT: api/case/5
        [HttpPut("{id}")]
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

        // DELETE: api/case/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCase(int id)
        {
            var caseItem = await _caseService.GetCaseByIdAsync(id);
            if (caseItem == null)
                return NotFound();

            await _caseService.DeleteCaseAsync(id);
            return NoContent();
        }
    }
}
