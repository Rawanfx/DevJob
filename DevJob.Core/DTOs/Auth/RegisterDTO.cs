using System.ComponentModel.DataAnnotations;

namespace DevJob.Application.DTOs.Auth
{
    public class RegisterDTO
    {
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [EmailAddress(ErrorMessage ="Email not valid")]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Compare("Password")]
        [Required]
        public string ConfirmPassword { get; set; }
        public string? Bio { get; set; }
    }
}
