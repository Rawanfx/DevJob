using System.ComponentModel.DataAnnotations;

namespace DevJob.Application.DTOs.Auth
{
    public class ForgetPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string ClientUri { get; set; }
    }
}
