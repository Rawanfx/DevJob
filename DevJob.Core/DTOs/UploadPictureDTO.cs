using Microsoft.AspNetCore.Http;

namespace DevJob.Application.DTOs
{
    public  class UploadPictureDTO
    {
        public string userId { get; set; }
       public IFormFile logo { get; set; }
    }
}
