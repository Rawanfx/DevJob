using Microsoft.AspNetCore.Http;

namespace DevJob.Application.DTOs.Cvs
{
    public class UploadCVDto
    {
       public IFormFile file { get; set; }
    }
}
