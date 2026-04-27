using DevJob.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace DevJob.Application.DTOs.Company
{
    public class PostJobDTO
    {
        [Required]
        [StringLength(100)]
        [MinLength(3)]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Location { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime DeadLine { get; set; }

        public int MinimumExperience { get; set; }
 
        public int? MaximumExperience { get; set; }
        [Required]
        public JobType JobType { get; set; }
        [Required]
        public List<string> skills { get; set; }
        public JobLevel JobLevel { get; set; }
        [Required]
        public EmploymentType EmploymentType { get; set; }

    }
}
