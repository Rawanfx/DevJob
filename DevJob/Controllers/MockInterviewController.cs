using DevJob.Application.DTOs.MockInterview;
using DevJob.Application.ServiceContract;
using DevJob.Infrastructure.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Cryptography;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Security.Claims;

namespace DevJob.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MockInterviewController : ControllerBase
    {
        private readonly IMockInterviewService mockInterviewService;
        private readonly IInterviewReportService interviewReportService;
        private readonly IStorageService storageService;
        public MockInterviewController(IMockInterviewService mockInterviewService
            ,IInterviewReportService interviewReportService
            ,IStorageService storageService) { 
            this.mockInterviewService = mockInterviewService;
            this.interviewReportService = interviewReportService;
            this.storageService = storageService;
        }
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
        [HttpPost("submit-get-nextQuestion/{videoId}")]
        [Authorize(Roles ="Developer")]
        public async Task<IActionResult>SubmitAngGetNextQ (Guid videoId)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await mockInterviewService.SubmitAnswerAndGetNextQuestion(videoId, userid);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpPost("reports/{mockInterviewId}")]
        [Authorize(Roles ="Developer")]
        public async Task<IActionResult> Reports (int mockInterviewId)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await interviewReportService.GenerateReportAsync(mockInterviewId, userid);
           
            return Ok(result);
        }
        [HttpGet("video-url/{videoId}")]
        public async Task<IActionResult>GetUrl (string videoId)
        {
            var result = storageService.GetCleanVideoUrl(videoId);
            return Ok(result);
        }
    }
}
