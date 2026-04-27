using System.ComponentModel.DataAnnotations;

namespace DevJob.Application.DTOs.User
{
    public class UserProfileDTO
    {
        public string? Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public string Location { get; set; }
        [Url(ErrorMessage = "Invalid LinkedIn URL format")]
        public string LinkedIn { get; set; }
        [Url(ErrorMessage = "Invalid GitHub URL format")]
        public string Github { get; set; }
    }
}
