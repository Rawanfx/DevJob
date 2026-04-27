using AutoMapper;
using DevJob.Application.DTOs.Auth;
using DevJob.Domain.Entities;
using System.Runtime.Intrinsics.Arm;

namespace DevJob.Application.Mappers
{
    public class ApplicationUserFromCompanyRegister:Profile
    {
       
        public ApplicationUserFromCompanyRegister()
        {
            CreateMap<CompanyRegisterDTO, ApplicationUser>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));


        }
    }
}
