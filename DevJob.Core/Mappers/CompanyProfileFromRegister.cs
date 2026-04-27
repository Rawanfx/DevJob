using AutoMapper;
using DevJob.Application.DTOs.Auth;
using DevJob.Domain.Entities;
namespace DevJob.Application.Mappers
{
    public class CompanyProfileFromRegister:Profile
    {
        public CompanyProfileFromRegister()
        {
            CreateMap<CompanyRegisterDTO, CompanyProfile>()
                .ForMember(dest => dest.SerailNumber, opt => opt.MapFrom(src => src.SerailNumber))
                .ForMember(dest=>dest.CompanyName,opt=>opt.MapFrom(src=>src .CompanyName))
      ;
        }
    }
}
