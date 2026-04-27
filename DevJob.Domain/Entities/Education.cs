using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace DevJob.Domain.Entities
{
    public class Education
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string InstitutionName { get; set; }
        public double Degree { get; set; }
        public string FieldOfStudy { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [ForeignKey(nameof(User))]
        public int UserID { get; set; }
        public UserCvData User { get; set; }
        [ForeignKey(nameof(CV))]
        public int cvId { get; set; }
        public CV CV { get; set; }
    }
}
