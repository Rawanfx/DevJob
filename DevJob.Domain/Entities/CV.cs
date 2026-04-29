using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace DevJob.Domain.Entities
{
    public class CV
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FileNameAzure { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        [ForeignKey(nameof (ApplicationUser))]
        [MaxLength(450)]
        public string UserId { get; set; }
        public UserProfile UserProfile { get; set; }
        [ForeignKey(nameof (UserProfile))]
        public int userprofileId { get; set; }
        public string Hash { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection <Projects> Projects { get; set; }
        public ICollection<ProjectSkills> ProjectSkills { get; set; }
        public ICollection<Skills> Skills { get; set; }
        public ICollection <UserCvData> UserCvDatas { get; set; }
        
    }
}
