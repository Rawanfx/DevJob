using Microsoft.AspNetCore.Http;

namespace DevJob.Application.DTOs.Company
{
    public class UploadLogoDTO
    {
        public IFormFile Logo { get; set; }
        public string userId { get; set; }
    }
}
