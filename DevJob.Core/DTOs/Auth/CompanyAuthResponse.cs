using System.ComponentModel.DataAnnotations;

namespace DevJob.Application.DTOs.Auth
{
    public class CompanyAuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
