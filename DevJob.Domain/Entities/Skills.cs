using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevJob.Domain.Entities
{
    public class Skills
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(255)]
        public string SkillName { get; set; }
        public ICollection<UserSkills> UserSkills { get; set; }
        public ICollection<RequiredSkills> requiredSkills { get; set; }
        public ICollection <ProjectSkills> ProjectSkills { get; set; }
      //  public UserCvData User { get; set; }
        [ForeignKey(nameof(CV))]
        public int? cvId { get; set; }
        public CV CV { get; set; }
    }
}
