using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DevJob.Domain.Entities
{
    public class Projects
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [JsonPropertyName("title")]
        public string ProjectName { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [Url]
        public string? Url { get; set; }
        [ForeignKey(nameof(user))]
        public int UserId { get; set; }
        public UserCvData user { get; set; }
        public ICollection<ProjectSkills> ProjectSkills { get; set; }
        [ForeignKey(nameof(CV))]
        public int cvId { get; set; }
        public CV CV { get; set; }
    }
}
