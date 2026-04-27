using DevJob.Domain.Enums;

namespace DevJob.Application.DTOs.Jobs
{
    public class RecommendedJobDto
    {
        public int job_id { get; set; }
        public string Title { get; set; }
        public double MatchScore { get; set; }
        public string Desctiption { get; set; }
        public string Location { get; set; }
        public string CompanyName { get; set; }
        public string? PostedAt { get; set; }
        public JobType? JobType { get; set; }
        public JobLevel? JobLevel { get; set; }
        public EmploymentType? EmploymentType { get; set; }
        public DateTime? DeadLine { get; set; }
        public string apply_Link { get; set; }

    }
}
