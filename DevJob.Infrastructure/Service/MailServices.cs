using DevJob.Application.DTOs.Auth;
using DevJob.Application.ServiceContract;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
namespace DevJob.Infrastructure.Service
{
    public class MailServices : IMailServices
    {
        private readonly MailSetting mailSetting;
        private readonly SmtpClient smtpClient;
        public MailServices(IOptions<MailSetting> options)
        {
            mailSetting = options.Value;
        }

        public async Task SendEmailAsync(string mailTo, string? subject, List<IFormFile> attachments,string body)
        {
            var message = new MimeMessage();

            // From
            message.From.Add(new MailboxAddress("DevJob", mailSetting.Email));

            // To
            message.To.Add(new MailboxAddress("", mailTo));

            message.Subject = subject ?? string.Empty;

            var builder = new BodyBuilder();

            // Attachments
            if (attachments != null)
            {
                foreach (var file in attachments)
                {
                    if (file.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        await file.CopyToAsync(ms);
                        builder.Attachments.Add(file.FileName, ms.ToArray());
                    }
                }
            }

       
            builder.HtmlBody = body;

            message.Body = builder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            await smtp.ConnectAsync(mailSetting.Host, mailSetting.Port, MailKit.Security.SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(mailSetting.Email, mailSetting.Password);

            await smtp.SendAsync(message);

            await smtp.DisconnectAsync(true);
        }

    }
}
