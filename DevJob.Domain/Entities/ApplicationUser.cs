
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Primitives;
namespace DevJob.Domain.Entities
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
        public DateOnly CreatedAt { get; set; }
        public string? RefreshToken { get; set; }
        public string? DeviceId { get; set; }
        public DateTime RefreshTokenExpirationDate { get; set; }
        public IList<CV> CVs { get; set; }
        public IList<Projects> Projects { get; set;
        }
        
        public ICollection<UserCvData> UserCvData { get; set; }


    }
}
