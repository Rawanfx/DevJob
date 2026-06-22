using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevJob.Domain.Entities
{
    public class InterviewVideo
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public VideoStatus Status { get; set; } = VideoStatus.PendingUpload;
        [ForeignKey(nameof (MockInterviewQuestion))]
        public int QuestionId { get; set; }
        public MockInterviewQuestion MockInterviewQuestion { get; set; }
        public required string StorageKey { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ReportJson { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
