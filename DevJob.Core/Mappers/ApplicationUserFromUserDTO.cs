using AutoMapper;
using DevJob.Application.DTOs.Auth;
using DevJob.Domain.Entities;

namespace DevJob.Application.Mappers
{
    public class ApplicationUserFromUserDTO : Profile
    {
        public ApplicationUserFromUserDTO()
        {
            CreateMap<RegisterDTO, ApplicationUser>()
                .ForMember(src => src.Email, opt => opt.MapFrom(x => x.Email))
                .ForMember(src => src.UserName, op => op.MapFrom(x => x.Email))
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.FirstName + " " + x.LastName))
                .ForMember(x => x.PasswordHash, opt => opt.Ignore());

        }
    }
}
