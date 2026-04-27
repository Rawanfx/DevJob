using AutoMapper;
using DevJob.Application.DTOs.Company;
using DevJob.Domain.Entities;
namespace DevJob.Application.Mappers
{
    public class JobFromPostRequestDTO:Profile
    {
        public JobFromPostRequestDTO()
        {
            CreateMap<PostJobDTO, Job>()
                .ForMember(x => x.JobLevel, opt => opt.MapFrom(x => x.JobLevel))
                .ForMember(x => x.Description, opt => opt.MapFrom(x => x.Description))
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.JobType, opt => opt.MapFrom(x => x.JobType))
                .ForMember(x => x.Location, opt => opt.MapFrom(x => x.Location))
                .ForMember(x => x.EmploymentType, opt => opt.MapFrom(x => x.EmploymentType))
                .ForMember(x => x.MaximumExperience, opt => opt.MapFrom(x => x.MaximumExperience))
                .ForMember(x => x.MinimumExperience, opt => opt.MapFrom(x => x.MinimumExperience))
                .ForMember(x => x.Title, opt => opt.MapFrom(x => x.Title))
                .ForMember(x => x.DeadLine, opt => opt.MapFrom(x => x.DeadLine));
        }
    }
}
