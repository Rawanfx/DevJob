using AutoMapper;
using DevJob.Application.DTOs.Auth;
using DevJob.Domain.Entities;

namespace DevJob.Application.Mappers
{
    public class UserProfileFromRegisterDTO:Profile
    {
        public UserProfileFromRegisterDTO()
        {
            CreateMap<RegisterDTO, UserProfile>()
                .ForMember(dest => dest.ApplicationUser, opt => opt.Ignore());
        }
    }
}
