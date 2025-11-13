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
        private readonly ICitizenDirectoryClient _citizenClient;
        

        public AlertsController(INotificationService notifications,ICitizenDirectoryClient citizenDirectoryClient )
        {
            _notifications = notifications;
            _citizenClient = citizenDirectoryClient;
        }

        //[HttpPost("community-alert")]
        //[Authorize(Policy = "AdminOnly")]
        //public async Task<IActionResult> SendAlert([FromBody] CreateAlertDto dto)
        //{
        //    await _notifications.SendCommunityAlertAsync(dto.City, dto.Title, dto.Message);
        //    return Ok(new { message = "Alert sent to subscribed citizens." });
        //}

        [HttpPost("community-alert-from-citizens")]
        public async Task<IActionResult> SendCommunityAlertFromCitizenService(
           [FromBody] CommunityAlertFromCitizenServiceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //var filter = new CitizenEmailFilterRequestDto
            //{
            //    City = dto.City

            //};
            await _notifications.SendCommunityAlertAsync(dto.City, dto.Title, dto.Message);


            return Ok(new
            {
                message = "Community alert sent",
               // recipients = emails.Count
            });
        }
    }
}
