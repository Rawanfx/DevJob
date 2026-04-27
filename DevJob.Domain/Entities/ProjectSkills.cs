
using System.ComponentModel.DataAnnotations.Schema;

namespace DevJob.Domain.Entities
{
    public class ProjectSkills
    {
        [ForeignKey(nameof (Projects))]
        public int ProjectId { get; set; }
        public Projects Projects { get; set; }
        [ForeignKey(nameof(Skills))]
        public int SkillId { get; set; }
        public Skills Skills { get; set; }
        [ForeignKey(nameof(CV))]
        public int cvId { get; set; }
        public CV CV { get; set; }
    }
}
