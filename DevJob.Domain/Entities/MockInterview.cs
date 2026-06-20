using DevJob.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevJob.Domain.Entities
{
    public class MockInterview
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof (User))]
        public string UserId { get; set; }
        [ForeignKey (nameof (CV))]
        public int CvId { get; set; }
        [ForeignKey(nameof (Job))]
        public int? JobId { get; set; }
          
        public string Track { get; set; }
        public string Level { get; set; }
        public InterviewStatus Status { get; set; }
        public float? Score { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public string? VideoUrl { get; set; }
        public ApplicationUser User { get; set; }
        public CV CV { get; set; }
        public Job Job { get; set; }
        public ICollection<MockInterviewQuestion> Questions { get; set; }
    }
}
