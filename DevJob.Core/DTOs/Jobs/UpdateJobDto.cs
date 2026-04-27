using DevJob.Domain.Enums;

namespace DevJob.Application.DTOs.Jobs
{
    public class UpdateJobDto
    {
        public string Desctiption { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public JobType JobType { get; set; }
        public JobLevel JobLevel { get; set; }
        public EmploymentType EmploymentType { get; set; }
        public DateTime? DeadLine { get; set; }
    }
}
