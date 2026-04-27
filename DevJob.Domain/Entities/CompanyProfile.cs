
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DevJob.Domain.Entities
{
    public class CompanyProfile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string SerailNumber { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        [ForeignKey(nameof(applicationUser))]
        public string ApplicationUser { get; set; }
        public string CompanyName { get; set; }
        public string? Website { get; set; }
        public string? Logo { get; set; }
        [JsonIgnore]
        public ApplicationUser applicationUser { get; set; }

    }
}
