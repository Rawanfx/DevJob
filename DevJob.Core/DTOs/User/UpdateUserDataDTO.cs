using AutoMapper;

namespace DevJob.Application.DTOs.User
{
    public class UpdateUserDataDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public UserProfileDTO data { get; set; }
    }
}
