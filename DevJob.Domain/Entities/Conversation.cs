using System.ComponentModel.DataAnnotations.Schema;

namespace DevJob.Domain.Entities
{
    public class Conversation
    {
        public int Id { get; set; }
        public UserCvData UserCvData1 { get; set; }
        [ForeignKey(nameof(UserCvData1))]
        public int UserId { get; set; }
        public Job Job1 { get; set; }
        [ForeignKey(nameof (Job1))]
        public int jobId { get; set; }
        public CompanyProfile CompanyProfile1 { get; set; }
        [ForeignKey(nameof (CompanyProfile1))]
        public int CompanyId { get; set; }
        public DateTime date { get; set; }
    }
}
