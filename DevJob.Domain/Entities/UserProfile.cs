
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevJob.Domain.Entities
{
    public class UserProfile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? PictureLink { get; set; }
        public string? ProfileLink { get; set; }
      
        public ApplicationUser ApplicationUser { get; set; }
        [ForeignKey(nameof (ApplicationUser))]
        public string userId  { get; set; }
        public string? Github { get; set; }
        public string? LinkedIn { get; set; }
       
        public ICollection<CV>? cVs { get; set; }
      
    }
}
