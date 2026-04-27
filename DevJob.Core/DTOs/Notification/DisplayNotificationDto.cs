using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevJob.Application.DTOs.Notification
{
    public class DisplayNotificationResultDto
    {
        public bool Success { get; set; }
        public List<DisplayNotificationDto> displayNotificationDtos { get; set; }
        public string Message { get; set; }
    }
    public class DisplayNotificationDto
    {
        public string Title { get; set; }
        public string Message { get; set; }
     public DateTime CreatedDate { get; set; }
        public bool IsRead { get; set; }
    }
}
