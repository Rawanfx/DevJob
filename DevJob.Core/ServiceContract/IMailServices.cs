using Microsoft.AspNetCore.Http;
namespace DevJob.Application.ServiceContract
{
    public interface IMailServices
    {
        Task SendEmailAsync(string mailTo, string? subject, List<IFormFile> attachments,string body);
    }
}
