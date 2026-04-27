using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevJob.Domain.Entities
{
    public class Experience
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndTime { get; set; }
        public string? Description { get; set; }
        [ForeignKey (nameof (User))]
        public int UserId { get; set; }
        public UserCvData User { get; set; }
        [ForeignKey(nameof(CV))]
        public int cvId { get; set; }
        public CV CV { get; set; }
    }
}
