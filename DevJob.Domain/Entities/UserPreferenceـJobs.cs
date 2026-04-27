
using DevJob.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevJob.Domain.Entities
{
    public class UserPreferenceـJobs
    {
        public int Id { get; set; }
        public JobType JobType { get; set; }
      
        public UserCvData UserCvData1 { get; set; }
        [ForeignKey(nameof (UserCvData1))]
        public int userId { get; set; }
    }
}
