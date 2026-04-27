using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevJob.Domain.Entities
{
    public class Trainning
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Organization { get; set; }
        public string Description { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        [ForeignKey(nameof (UserCvData))]
        public int UserId { get; set; }
        public UserCvData UserCvData { get; set; }
        [ForeignKey(nameof(CV))]
        public int cvId { get; set; }
        public CV CV { get; set; }
    }
}
