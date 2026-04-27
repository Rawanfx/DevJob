using DevJob.Domain.Enums;

namespace DevJob.Application.DTOs.Jobs
{
    public class UserPrefareDto
    {
        public List<JobType> jobTypes { get; set; }
        public JobLevel JobLevel { get; set; }
        public List<string> skills { get; set; }
        public decimal MinimumSalar { get; set; }
    }
}
