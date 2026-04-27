using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevJob.Domain.Entities
{
    public class UserSkills
    {
        public UserCvData UserCvData { get; set; }
        [ForeignKey(nameof(UserCvData))]
        public int UserId { get; set; }
        [ForeignKey(nameof(Skills))]
        public int SkillId { get; set; }
        public Skills Skills { get; set; }
        [ForeignKey(nameof(CV))]
        public int cvId { get; set; }
        public CV CV { get; set; }

    }
}
