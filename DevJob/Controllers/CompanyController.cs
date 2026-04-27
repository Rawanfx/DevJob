using DevJob.Application.DTOs;
using DevJob.Application.DTOs.Company;
using DevJob.Application.ServiceContract;
using DevJob.Infrastructure.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Security.Claims;
namespace DevJob.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly IJobServices jobServices;
        private readonly ICompanyServices companyServices;
       // private string UserId ()
        public CompanyController(ICompanyServices companyServices,IJobServices jobServices)
        {
            this.companyServices = companyServices;
            this.jobServices = jobServices;
        }
        [HttpPost("Update-profile")]
        [Authorize(Roles = "Company")]
        public async Task< IActionResult>UpdateProfile([FromForm]UpdateCompanyProfileDTO profileDTO, [FromForm] UploadPictureDTO pictureDTO)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            profileDTO.Id = userId;
            pictureDTO.userId = userId;
            var response = await companyServices.Update(profileDTO, pictureDTO);
            if (response.Success)
                return Ok(response);
            return BadRequest(response);
        }
        [Authorize(Roles = "Company")]
        [HttpGet("Get-company-data")]
        public async Task<IActionResult> getCompany()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await companyServices.GetCompany(userId);
            return Ok(result);
        }
        [HttpGet("applicant-search")]
        [Authorize(Roles = "Company")]
       public async Task<IActionResult> ApplicantSearch (string item,int jobId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await companyServices.ApplicantSearch(userId, item, jobId);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
      
    }
}
