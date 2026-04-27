using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevJob.Domain.Entities
{
    public class RecommendedJobs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey(nameof (Job1))]
        public int jobId { get; set; }
        public Job Job1 { get; set; }
        [ForeignKey(nameof(UserCvData1))]
        public int userId { get; set; }

        public  UserCvData UserCvData1 { get; set; }
        public double MatchScore { get; set;}
        public bool IsSent { get; set; }
        
    }
}
