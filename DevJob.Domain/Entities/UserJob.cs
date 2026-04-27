using DevJob.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevJob.Domain.Entities
{
    public class UserJob
    {
        public UserCvData UserCvData { get; set; }
        public Job job { get; set; }
        [ForeignKey(nameof (UserCvData))]
        public int userId { get; set; }
        [ForeignKey(nameof (Job))]
        public int jobID { get; set; }
        public DateOnly AppliedAt { get; set; }
        public Status Status { get; set; }
        public int cvId { get; set; }
        public CV CV1 { get; set; }

    }
}
