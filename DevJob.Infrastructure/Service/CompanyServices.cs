using AutoMapper;
using DevJob.Application.DTOs;
using DevJob.Application.DTOs.Company;
using DevJob.Application.DTOs.User;
using DevJob.Domain.Entities;
using DevJob.Application.ServiceContract;
using DevJob.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DevJob.Application.DTOs.Jobs;
using DevJob.Application.Repository_Contract;
namespace DevJob.Infrastructure.Service
{
    public class CompanyServices : ICompanyServices
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUploadToAzure uploadToAzure;
        private readonly ICompanyRepository companyRepository;
        private readonly IJobRepository jobRepository;
        private readonly IUserSkillsRepository userSkillsRepository;
        public CompanyServices(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager
            , IUploadToAzure uploadToAzure
            , ICompanyRepository companyRepository
            , IJobRepository jobRepository
            ,IUserSkillsRepository userSkillsRepository
            )
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.uploadToAzure=uploadToAzure;
            this.companyRepository = companyRepository;
            this.jobRepository = jobRepository;
            this.userSkillsRepository = userSkillsRepository;
        }

        public async Task<GetApplicantResultDto> ApplicantSearch(string companyId, string item,int jobId)
        {
            var company = await unitOfWork.CompanyProfile.FirstOrDefaultAsync(x => x.ApplicationUser == companyId);
            if (company == null)
                return new GetApplicantResultDto() { Success=false,Message="Company Not Found"};
            var job = await unitOfWork.Jobs.FirstOrDefaultAsync(x => x.Id == jobId && x.CompanyId == company.Id);
            if (job == null)
                return new GetApplicantResultDto() { Success = false, Message = "Job Not Found" };
            var applicants = await companyRepository.ApplicantSearch(item, jobId);
            var applicantIds = applicants.Select(x => x.userId).ToHashSet();
            var skills = await userSkillsRepository.GetUserSkills(applicantIds);

            var score = await jobRepository.GetScoreForApplicant(jobId);
          

            foreach (var i in applicants)
            {
                i.MatchScore = score.GetValueOrDefault(i.userId);
                i.skillName = skills.GetValueOrDefault(i.userId);
            }
            return new GetApplicantResultDto()
            {
                Success = true,
                getApplicantDtos = applicants,
            };
        }

        public async Task< int?> CompanyId(string userId)
        {
            var x = await unitOfWork.CompanyProfile.FirstOrDefaultAsync(x => x.ApplicationUser == userId);
            return x?.Id;
        }

        public async Task<GetCompanyDTO> GetCompany(string id)
        {
            //get record from application user
            //get record from company profile
            ApplicationUser? company = await userManager.FindByIdAsync(id);
            if (company == null)
                return new GetCompanyDTO();

            CompanyProfile? companyProfile = await unitOfWork.CompanyProfile.FirstOrDefaultAsync(x => x.ApplicationUser == id);
            return new GetCompanyDTO()
            {
                Description = companyProfile?.Description,
                Email = company.Email,
                Location = companyProfile.Location,
                Name = company.Name,
                Website = companyProfile.Website
            };
        }

        public async Task<UpdateCompanyProfileResult> Update(UpdateCompanyProfileDTO profileDTO,UploadPictureDTO logo)
        {
            //logic 
            //1- get company from Db
            //2- update data
            //3-save update in db
           
                ApplicationUser? applicationUser = await userManager.FindByIdAsync(profileDTO.Id!);
                if (applicationUser == null)
                {
                    return new UpdateCompanyProfileResult()
                    {
                        Success = false,
                        Message = "Invalid Id",
                    };
                }

                CompanyProfile? profile = await unitOfWork.CompanyProfile.FirstOrDefaultAsync(x => x.ApplicationUser == profileDTO.Id);

            if (profile == null)
                {
                    return new UpdateCompanyProfileResult()
                    {
                        Success = false,
                        Message = "Invalid Id",
                    };
                }

                //update
                if (profileDTO.Location != null)
                    profile.Location = profileDTO.Location;

                if (profileDTO.Description != null)
                    profile.Description = profileDTO.Description;

                if (profileDTO.Website != null)
                    profile.Website = profileDTO.Website;

                applicationUser.Name = profileDTO.Name;
                UploadLogoDTO logoDTO = new UploadLogoDTO();
                logoDTO.Logo = logo.logo;
                var result = await UploadLogo(logoDTO);
                profile.Logo = result.Url;
                await userManager.UpdateAsync(applicationUser);
                await unitOfWork.SaveChangesAsync();
            return new UpdateCompanyProfileResult()
            {
                Success = true,
                Message = "Company profile Update Succesfully",


            };
        }

        public async Task<UploadLogoResponse> UploadLogo(UploadLogoDTO uploadLogoDTO)
        {
            var url = await uploadToAzure.UploadFileToAzure(uploadLogoDTO.Logo,uploadLogoDTO.userId);
            return new UploadLogoResponse() { Success = true, Message = "Logo Uploaded succesuflly" };
              //  ,Url=url  
        }
    }
}
