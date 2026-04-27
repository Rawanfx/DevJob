using DevJob.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DevJob.Domain.Entities
{
    public class Job
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? PostedAt { get; set; }
        public int? MinimumExperience { get; set; }
        [ForeignKey(nameof (companyProfile))]
        public int? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public int? MaximumExperience { get; set; }
        public JobType JobType { get; set; }
        public bool IsProcessed { get; set; } = false;
        public JobLevel JobLevel { get; set; }
        public EmploymentType EmploymentType { get; set; }
        public bool IsActive { get; set; } = true;
        public string Hash { get; set; }
        public ICollection< RequiredSkills >RequiredSkills { get; set; }
        public IList<UserJob> UserJobs { get; set; }
        public CompanyProfile companyProfile { get; set; }
        public DateTime? DeadLine { get; set; }
        public bool Local { get; set; }
        public string? Source { get; set; }
        public string? ApplyLink { get; set; }
        public string? JobIdExtension { get; set; }
    }
}
