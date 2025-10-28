using Crime_Management_System.Models;
using Crime_Management_System.Services;
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
            var cases = await _caseService.GetAllAsync();
            return Ok(cases);
        }

        // GET: api/case/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCase(int id)
        {
            var caseItem = await _caseService.GetByIdAsync(id);
            if (caseItem == null)
                return NotFound();

            return Ok(caseItem);
        }

        // POST: api/case
        [HttpPost]
        public async Task<IActionResult> CreateCase([FromBody] Case caseItem)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _caseService.AddAsync(caseItem);
            return CreatedAtAction(nameof(GetCase), new { id = caseItem.Id }, caseItem);
        }

        // PUT: api/case/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCase(int id, [FromBody] Case caseItem)
        {
            if (id != caseItem.Id)
                return BadRequest("Case ID mismatch");

            var existingCase = await _caseService.GetByIdAsync(id);
            if (existingCase == null)
                return NotFound();

            await _caseService.UpdateAsync(caseItem);
            return NoContent();
        }

        // DELETE: api/case/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCase(int id)
        {
            var caseItem = await _caseService.GetByIdAsync(id);
            if (caseItem == null)
                return NotFound();

            await _caseService.DeleteAsync(id);
            return NoContent();
        }
    }
}
