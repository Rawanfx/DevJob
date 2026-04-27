using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevJob.Domain.Entities
{
    public class UserPrefernce_Skills
    {
        public int Id { get; set; }
        [ForeignKey(nameof(UserCvData1))]
        public int userCvData { get; set; }
        [ForeignKey(nameof(Skills1))]
        public int SkillId { get; set; }
        public Skills Skills1 { get; set; }
        public UserCvData UserCvData1 { get; set; }
    }
}
