using DevJob.Application.DTOs.Company;
using DevJob.Application.DTOs.Jobs;
using DevJob.Application.ServiceContract;
using DevJob.Infrastructure.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace DevJob.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IJobServices jobService;
        public JobsController(IJobServices jobService)
        {
            this.jobService = jobService;
        }
        [Authorize(Roles = "Company")]
        [HttpPost("add-job")]

        public async Task<IActionResult> AddJob(PostJobDTO postJobDTO)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
          
            var res = await jobService.AddJob(postJobDTO,userId);
            if (res.Success)
                return Ok(res);
            return BadRequest(res);
        }
        [HttpGet]
        [Authorize(Roles = "Developer")]
        public  async Task< IActionResult> Display()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await jobService.GetJobsFromSerpApi(userId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [Authorize(Roles ="Company")]
        [HttpPut("update")]
        public async Task<IActionResult> Update(int jobId,[FromBody]UpdateJobDto updateJobDto)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await jobService.Update(jobId, user, updateJobDto);
            if (response.Success)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpDelete]
        [Authorize(Roles ="Company")]
        public async Task<IActionResult> Delete (int jobId)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await jobService.Delete(jobId, user);
            if (response.Success)
                return Ok(response);
            return BadRequest(response);
        }
        [Authorize(Roles ="Company")]
        [HttpGet("get-all-jobs")]
        public async Task<IActionResult>AllJobs()
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await jobService.AllJobs(user);
            if (response.Count == 0)
                return NoContent();
            return Ok(response);
        }
        [HttpPost("Apply")]
        [Authorize (Roles ="Developer")]
        public async Task<IActionResult> apply(int jobId,int cvId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await jobService.Apply(jobId, userId,cvId);
            if (response.Success)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("Recommended-jobs")]
        [Authorize(Roles ="Developer")]
        public async Task< IActionResult > DisplayRecommendedJobs()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await jobService.DisplayRecommendedjobs(userId);
          
            return Ok(response);
        }
        [HttpGet("get-all-applicants")]
        [Authorize (Roles = "Company")]
        public async Task<IActionResult> GetAllApplicants(int jobId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var res = await jobService.GetApplicants(jobId, userId);
           if (res.Success)
            {
                if (res.getApplicantDtos.Count == 0)
                    return NoContent();
                return Ok(res);
            }
            return BadRequest(res);
        }
        [HttpPut("Update-state")]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> UpdateState (UpdateStatusDto updateStatusDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res =await jobService.UpdateStatus(updateStatusDto,userId);

            if (res.Success)
                return Ok(res);
            return BadRequest(res);
        }
        [HttpGet("all-skills")]
        public async Task<IActionResult> GetAllSkills()
        {
          var res=  await jobService.GetAllSkills();
            if (res.Count == 0)
                return NoContent();
            return Ok(res);
        }
        [HttpGet("applicant-count")]
        [Authorize(Roles = "Company")]
        public async Task< IActionResult> ApplicantCount([FromQuery]int jobId)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var res =await jobService.ApplicantsCount(user,jobId);
            if (res.Success)
            {
              
                return Ok(res);
            }
            return BadRequest(res);
        }
        [HttpPost("user-prefare")]
        [Authorize(Roles = "Developer")]
        public async Task<IActionResult> AddUserPrefare(UserPrefareDto userPrefareDto)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var res = await jobService.AddUserPrefare(userPrefareDto, user);
            if (res.Success)
                return Ok(res);
            return BadRequest(res);
        }
        [HttpGet("company-count")]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> CompanyCount()
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var res = await jobService.CompanyCount(user);
            if (res.Success)
                return Ok(res);
            return BadRequest(res);
        }
        [HttpPost("add-saved-job")]
        [Authorize(Roles ="Developer")]
        public async Task< IActionResult> AddSavedJob(AddSavedJobsDto addSavedJobsDto)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var res = await jobService.AddSavedJobs(addSavedJobsDto,user);
            if (res.Success)
                return Ok(res);
            return BadRequest(res);
        }
        [HttpGet("display-saved-jobs")]
        [Authorize(Roles = "Developer")]
        public async Task< IActionResult>DisplaySavedJobs ()
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var res = await jobService.DisplaySavedJob(user);
            if (res.Success)
                return Ok(res);
            return BadRequest(res);
        }
        [HttpGet("all-jobs")]
        public async Task<IActionResult> DisplayAllJobs()
        {
            var res =await jobService.DisplayAllJobs();
            if (res.Count == 0)
                return NoContent();
            return Ok(res);
        }
        [HttpGet("job-search")]
        public async Task<IActionResult>JobSearch (string item)
        {
            var result = await jobService.JobSearch(item);
            return Ok(result);
        }
        [HttpGet("saved-job-search")]
        [Authorize(Roles = "Developer")]
        public async Task<IActionResult> SavedJobSearch(string item)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await jobService.SavedJobSearch(item, user);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
