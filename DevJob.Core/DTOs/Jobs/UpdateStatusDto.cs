using DevJob.Domain.Enums;

namespace DevJob.Application.DTOs.Jobs
{
    public class UpdateStatusDto
    {
        public int JobId { get; set; }
        public int UserId { get; set; }
        public Status status { get; set;
        }
    }
}
