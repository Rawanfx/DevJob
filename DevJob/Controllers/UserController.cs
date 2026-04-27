using DevJob.Application.DTOs.User;
using DevJob.Application.ServiceContract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevJob.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        public UserController(IUserService userService)
        {
            this.userService = userService;
        }
        [Authorize]
        [HttpGet("get-user-data")]
        public async Task< IActionResult> GetUserData()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
           var response=await userService.GetUserData(userId);
            return Ok(response);
        }
        [HttpGet("change-email")]
        [Authorize]
        public async Task <IActionResult>ConfirmEmail(string newEmail,string token)
        {
            string userEmail = User.FindFirst(ClaimTypes.Email).Value;
           var response= await userService.ChangeEmail(newEmail, token, userEmail);
            if (response.Success)
                return Ok("Email Changed successfully");
            return BadRequest(response.Message);
        }
        [HttpPut("Update-user-profile")]
        [Authorize(Roles ="Developer")]
        public async Task<IActionResult> UpdateUserProfile(UserProfileDTO userProfileDTO)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            userProfileDTO.Id = userId;
            var response=  await userService.UpdateUserData(userProfileDTO);
            if (response.Success)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("applicant-history-count")]
        [Authorize(Roles ="Developer")]
        public async Task<IActionResult> ApplicantHistoryCount()
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var res = await userService.ApplicantsCount(user);
            if (res.Success)
                return Ok(res);
            return BadRequest(res);
        }
        [HttpGet("applicant-history")]
        [Authorize (Roles ="Developer")]
        public async Task<IActionResult>ApplicantHistory()

        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var res = await userService.ApplicationHistoryData(user);
            if (res.Count == 0)
                return NoContent();
            return Ok(res);
        }
        [HttpGet("user-count")]
        [Authorize(Roles = "Developer")]
        public async Task< IActionResult >userCount()
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var res = await userService.UserCount(user);
            return Ok(res);
        }
    }
}
