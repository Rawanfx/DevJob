using DevJob.Domain.Enums;

namespace DevJob.Application.DTOs.User
{
    public class ApplicationHistoryData
    {
        public string JobName { get; set; }
        public int JobId { get; set; }
        public DateOnly ApplyDate { get; set; }
        public Status JobStatus { get; set; }
    }
}
