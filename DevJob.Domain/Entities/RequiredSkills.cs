using System.ComponentModel.DataAnnotations.Schema;

namespace DevJob.Domain.Entities
{
    public class RequiredSkills
    {
        [ForeignKey(nameof(Job))]
        public int JobId { get; set; }
        [ForeignKey(nameof(skills))]
        public int SkillId { get; set; }
        public Job Job { get; set; }
        public Skills skills { get; set; }
    }
}
