using DevJob.Application.ServiceContract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevJob.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService notificationService;
        public NotificationController(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }
        [HttpPut("device-id/{deviceId}")]
        [Authorize]
        public async Task<IActionResult > AddDeviceID(string deviceId)
        {
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var res =await notificationService.AddDeviceId(userId, deviceId);
            if (res.Success)
                return Ok(res);
            return BadRequest(res);
        }
        [HttpGet("all-notifications")]
        [Authorize]
        public async Task<IActionResult> DisplayAllNotifications()
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await notificationService.DisplayNotification(user);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
