using DevJob.Application.DTOs;
using DevJob.Application.DTOs.Chat;
using DevJob.Application.DTOs.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevJob.Application.ServiceContract
{
    public interface INotificationService
    {
        Task<ResultDto> AddDeviceId(string userId, string deviceId);
        Task<ResponseDto> SendNotification(string user, string title, string message);
        Task<bool> SaveNotification(int userId, string message, string title);
        Task<DisplayNotificationResultDto> DisplayNotification(string user);
    }
}
