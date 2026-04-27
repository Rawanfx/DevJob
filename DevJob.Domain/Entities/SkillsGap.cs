using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevJob.Domain.Entities
{
    public class SkillsGap
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey(nameof (ApplicationUser))]
        public int UserId { get; set; }
        public UserCvData ApplicationUser { get; set; }
        public string MissingSkill { get; set; }
        public string RecommendedCourses { get; set; }
    }
}
