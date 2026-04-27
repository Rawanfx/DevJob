using System.ComponentModel.DataAnnotations;

namespace DevJob.Application.DTOs.Auth
{
    public class ResetPasswordDTO
    {
        public string Email { get; set; }
        public string Token { get; set; }
        [Required(ErrorMessage ="Password is required")]
        public string Password { get; set; }
        [Required]
        [Compare ("Password",ErrorMessage ="Password and confirmation password do not match")]
        public string ConfirmPassword { get; set; }
    }
}
