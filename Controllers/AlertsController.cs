using Crime_Management_System.DTOs;
using Crime_Management_System.Servises;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Crime_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertsController : ControllerBase
    {
        private readonly INotificationService _notifications;

        public AlertsController(INotificationService notifications)
        {
            _notifications = notifications;
        }

        [HttpPost("community-alert")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> SendAlert([FromBody] CreateAlertDto dto)
        {
            await _notifications.SendCommunityAlertAsync(dto.City, dto.Title, dto.Message);
            return Ok(new { message = "Alert sent to subscribed citizens." });
        }
    }
}
