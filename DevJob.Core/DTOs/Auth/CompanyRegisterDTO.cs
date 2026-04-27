using System.ComponentModel.DataAnnotations;

namespace DevJob.Application.DTOs.Auth
{
    public class CompanyRegisterDTO
    {
        public string CompanyName { get; set; }
        public string SerailNumber { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
    }
}
