using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevJob.Domain.Entities
{
    public class UserCvData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey(nameof (user))]
        public string UserId { get; set; }
        public ApplicationUser user { get; set; }
        public string? Summary { get; set; }
        public string? LinkedInAccount { get; set; }
        public string? gitub { get; set; }
        public string? Email { get; set; }
        public ICollection<UserSkills> UserSkills { get; set; }
        public IList<UserJob> UserJobs { get; set; }
        [ForeignKey(nameof(CV))]
        public int cvId { get; set; }
        public CV CV { get; set; }
        public decimal? YearOfex { get; set; }
      
        public string ? JobTitle { get; set; }
    }
}
