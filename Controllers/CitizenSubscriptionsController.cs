using Crime_Management_System.Attributes;
using Crime_Management_System.DTOs;
using Crime_Management_System.Servises;
using Microsoft.AspNetCore.Mvc;

namespace Crime_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitizenSubscriptionsController : ControllerBase
    {
        private readonly ICitizenSubscriptionService _service;

        public CitizenSubscriptionsController(ICitizenSubscriptionService service)
        {
            _service = service;
        }

        // Subscribe to citizen notifications
        [HttpPost("subscribe")]
        [AllowAnonymous]
        public async Task<IActionResult> Subscribe([FromBody] CreateCitizenSubscriptionDto dto)
        {
            var result = await _service.CreateAsync(dto);

            var response = new CitizenSubscriptionResponseDto
            {
                Id = result.Id,
                FullName = result.FullName,
                Email = result.Email,
                City = result.City
            };

            return Ok(response);
        }

        // Unsubscribe from citizen notifications
        [HttpPost("unsubscribe")]
        [AllowAnonymous]
        public async Task<IActionResult> Unsubscribe([FromQuery] string email)
        {
            var success = await _service.UnsubscribeAsync(email);
            if (!success)
                return NotFound(new { message = "Subscription not found" });

            return Ok(new { message = "You have been unsubscribed." });
        }
    }
}
