using Microsoft.AspNetCore.Http;
namespace DevJob.Application.DTOs.Auth
{
    public class MailRequest
    {
        public string EmailTo { get; set; }
        public List<IFormFile>?Attachments { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
