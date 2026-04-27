
using DevJob.Application.ServiceContract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevJob.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CVController : ControllerBase
    {
        private readonly IUploadToAzure uploadToAzure;
        private readonly ICVServices cVServices;
        public CVController(
            IUploadToAzure uploadToAzure,
            ICVServices cVServices)
        {
            this.uploadToAzure = uploadToAzure;
            this.cVServices = cVServices;
        }
        [HttpPost("Upload")]
        [Authorize(Roles = "Developer")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string type =
                Path.GetExtension(file.FileName);
            if (type != ".pdf"  && type !=".docx")
                return BadRequest("Only PDF files are allowed.");
            var response = await cVServices.Upload(userId, file);
            
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        //[HttpPost("parse-cv")]
        //[Authorize]
        //public async Task<IActionResult> ParseCv([FromForm] ParseCvDto parseCvDto) 
        //{
        //    string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        //   var res= await cVServices.ParceCv(parseCvDto.url, parseCvDto.file, userId);
        //    if (res.Success)
        //        return Ok(res);
        //    return BadRequest(res);
        //}

        [HttpDelete]
        [Authorize(Roles = "Developer")]
        public async Task<IActionResult>Delete(int cvId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var res =await cVServices.DeleteCv(userId, cvId);
            if (res.Success) return Ok(res);
            return BadRequest(res);
        }
        [HttpGet("all-cv")]
        [Authorize(Roles = "Developer")]
        public async Task<IActionResult> GetAllCv()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var res = await cVServices.AllCv(userId);
            if (res.Count == 0)
                return NoContent();
            return Ok(res);
        }
    }
}
