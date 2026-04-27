using DevJob.Application.DTOs;
using DevJob.Application.DTOs.Chat;
using DevJob.Application.DTOs.Notification;
using DevJob.Domain.Entities;
using DevJob.Application.ServiceContract;
using DevJob.Infrastructure.Data;
using DevJob.Infrastructure.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;
namespace DevJob.Infrastructure.Service
{
    public class NotificationService : INotificationService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;
        private readonly IUnitOfWork unitOfWork;
        private readonly IHubContext<NotificationHub> hubContext;
        
        private readonly ILogger<NotificationService> logger;
        public NotificationService(UserManager<ApplicationUser> userManager
            , IHttpClientFactory httpClientFactory
            , IConfiguration configuration
            , IUnitOfWork unitOfWork
            , IHubContext<NotificationHub> hubContext
            ,ILogger<NotificationService> logger
           )
        {
            this.userManager = userManager;
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
            this.unitOfWork = unitOfWork;
            this.hubContext = hubContext;
            this.logger = logger;
        }
        public async Task<ResultDto> AddDeviceId(string userId, string deviceId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResultDto() { Success = false, Message = "User not Found" };
            user.DeviceId = deviceId;
           await userManager.UpdateAsync(user);
            await unitOfWork.SaveChangesAsync();
            return new ResultDto() { Success = true, Message = "Device Id added" };
        }
        public async Task<bool> SaveNotification(int user, string message, string title)
        {
            Notification notification = new()
            {
                Message = message,
                Title = title,
                date = DateTime.Now
            };
            if (await unitOfWork.UserCvData.AnyAsync(x => x.Id == user))
                notification.userId = user;
            else
                notification.CompanyId = user;
                try
                {
                    await unitOfWork.Notifications.AddAsync(notification);
                    await unitOfWork.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
        }
        public async Task<ResponseDto> SendNotification(string user, string title, string message)
        {
             var httpClient = httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", configuration["oneSignal:apiKey"]);
            var userDevice = await userManager.FindByIdAsync(user);
            if (userDevice == null)
                return new ResponseDto() { Success=false,Message="User not found"} ;
            var payLoad = new
            {
                app_id = configuration["oneSignal:appId"],
                include_aliases = new
                {
                    onesignal_id = new[] { userDevice.DeviceId },
                },
                target_channel = "push",
                headings = new { en = title },
                contents = new { en = message },
            };
            var response = await httpClient.PostAsJsonAsync(configuration["oneSignal:url"], payLoad);

            var responseBody = await response.Content.ReadAsStringAsync();
          logger.LogInformation($"Status Code: {response.StatusCode}");
          logger.LogInformation($"Response: {responseBody}");

            if (!response.IsSuccessStatusCode)
                throw new Exception(responseBody);

            return new ResponseDto() { Success=true,Message="Notification has been Sent"};

        }
        public async Task SendNotificationSignalR(string message, string title, string user)
        {
            await hubContext.Clients.Users(user).SendAsync("ReciveNotification", new {
                Title = title,
                Message = message,
                Date = DateTime.Now
            });
        }
        public async Task<DisplayNotificationResultDto> DisplayNotification(string user)
        {
           
            var userCvIds = await unitOfWork.UserCvData.Where(x => x.UserId == user).Select(x => x.Id).ToListAsync();
            var companyId = await unitOfWork.CompanyProfile.Where(x => x.ApplicationUser == user).Select(x => x.Id).FirstOrDefaultAsync();

            if (!userCvIds.Any() && companyId == 0)
                return new DisplayNotificationResultDto() { Success = false, Message = "User not found" };

            var notificationEntities = await unitOfWork.Notifications
                .Where(x => (x.userId != null && userCvIds.Contains(x.userId.Value)) || (x.CompanyId != null && x.CompanyId.Value == companyId))
                .OrderByDescending(x => x.date)
                .ToListAsync();

        
            var notificationDtos = notificationEntities.Select(x => new DisplayNotificationDto()
            {
                Title = x.Title,
                Message = x.Message,
                CreatedDate = x.date,
                IsRead = x.IsRead 
            }).ToList();

         
            var unread = notificationEntities.Where(x => !x.IsRead).ToList();
            if (unread.Any())
            {
                foreach (var n in unread)
                {
                    n.IsRead = true;
                }
                await unitOfWork.SaveChangesAsync();
            }

            
            return new DisplayNotificationResultDto() { Success = true, displayNotificationDtos = notificationDtos };
        }
    }
}
