using DevJob.Application.DTOs.MockInterview;
using DevJob.Application.ServiceContract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevJob.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MockInterviewController : ControllerBase
    {
        private readonly IMockInterviewService mockInterviewService;
        public MockInterviewController(IMockInterviewService mockInterviewService) => this.mockInterviewService = mockInterviewService;
        [Authorize(Roles ="Developer")]
        [HttpPost("start-interview")]
        public async Task<IActionResult> StartInterview(StartInterviewDto startInterviewDto)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await mockInterviewService.StartInterview(user, startInterviewDto);
            return Ok(result);

        }
        [HttpPost ("confirm-upload/{videoId}")]
        public async Task<IActionResult>ConfirmUpload (string videoId)
        {
            var result = await mockInterviewService.ConfirmUpload(videoId);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
