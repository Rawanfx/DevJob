using DevJob.Application.DTOs.Auth;
using DevJob.Application.DTOs.Jobs;
using DevJob.Application.DTOs.User;
using DevJob.Domain.Entities;
using DevJob.Application.ServiceContract;
using DevJob.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Bcpg;
using System.Net.Mail;
using DevJob.Domain.Enums;

namespace DevJob.Infrastructure.Service
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private AppDbContext context;
        private readonly IMailServices mailServices;
        private readonly IConfiguration configuration;
        public UserService(UserManager<ApplicationUser> userManager
            ,AppDbContext context
            ,IMailServices mailServices
            ,IConfiguration configuration)
        {
           this.userManager = userManager;
            this.context = context;
            this.mailServices = mailServices;
            this.configuration = configuration;
        }
        public async Task< UserDataDto> GetUserData(string Id)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(Id);
            if (user == null)
                return new UserDataDto();
            var userData = await context.UserCvDatas
                .Include(x=>x.CV)
                .Where(x => x.UserId == Id && !x.CV.IsDeleted)
                .Select(x=>x.Id)
                .ToListAsync();
            if (userData.Count>0)
            return new UserDataDto() { Name = user.Name, userId = userData ,AppUser=Id};
            var companyId = await context.Company.FirstOrDefaultAsync(x => x.ApplicationUser == Id);
            var userId = new List<int>() { companyId.Id};
            return new UserDataDto() {
                AppUser=Id,
                Name=companyId.CompanyName,
                userId=userId
            };
        }

        public async Task<UpdateUserDataDTO> UpdateUserData(UserProfileDTO dataDTO)
        {
            ApplicationUser? user= await  userManager.FindByIdAsync(dataDTO.Id!);
            if (user == null)
                return new UpdateUserDataDTO() { Success = false, Message = "User not found" };
            UserProfile? userProfile = await context.UserProfile.Where(x => x.userId == dataDTO.Id)
                .FirstOrDefaultAsync();
            //update email
            
                if (dataDTO.Email != user.Email)
                {
                    //generate token
                    var changeEmailToken = await userManager.GenerateChangeEmailTokenAsync(user, dataDTO.Email);
                    changeEmailToken = Uri.EscapeDataString(changeEmailToken);
                    //send token
                    var confirmLink = configuration["AppUrl"] + $"/api/User/ConfirmNewEmail?" +
                        $"newEmail={dataDTO.Email}&token={changeEmailToken}&email={user.Email}";

                    await mailServices.SendEmailAsync(dataDTO.Email, "DevJob", null, confirmLink);
                    var response = await ChangeEmail(dataDTO.Email, changeEmailToken, user.Email);
                    if (!response.Success)
                        return new UpdateUserDataDTO() { Success = false, Message = "Invalid email" };
                }
            //update location
            //if (dataDTO.Location != null)
            //    userProfile.Location = dataDTO.Location;
            ////update linkedin
            if (dataDTO.LinkedIn != null)
                userProfile.LinkedIn = dataDTO.LinkedIn;



            if (dataDTO.Name != null)
                    user.Name = dataDTO.Name;

            if (dataDTO.Github != null)
                userProfile.Github = dataDTO.Github;
            await context.SaveChangesAsync();
            await userManager.UpdateAsync(user);
            return new UpdateUserDataDTO()
                {
                    Success = true,
                    Message = "User profile updated successfully"
                    ,
                    data = new UserProfileDTO()
                    {
                       // Bio = userProfile.Bio,
                        Email = user.Email,
                        Github = userProfile.Github,
                        LinkedIn = userProfile.LinkedIn,
                        Name = user.Name
                    }
                };
            
        }
        public async Task<AuthResponse> ChangeEmail(string newEmail, string token,string email)
        {
            ApplicationUser? user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return new AuthResponse() { Message = "Email not valid" ,Success=false};
           var response= await userManager.ChangeEmailAsync(user, newEmail, token);
            if (response.Succeeded)
                return new AuthResponse()
                {Success = true,};
            throw new Exception(string.Join(',', response.Errors.Select(x => x.Description)));
            
        }

        public async Task<ApplicantHistoryCountResult> ApplicantsCount(string user)
        {
            //get userCvId
            var userCvData = await context.UserCvDatas.Where(x => x.UserId == user).Select(x=>x.Id).ToListAsync();
            if (userCvData == null)
                return new ApplicantHistoryCountResult() { Success = false, Message = "User not found" };
            var totalApply = await context.UserJobs.Where(x => userCvData.Contains(x.userId)).CountAsync();
            var interview = await context.UserJobs.Where(x => userCvData.Contains(x.userId) && x.Status == Status.Interview).CountAsync();
            var waiting = await context.UserJobs.Where(x => userCvData.Contains(x.userId) && x.Status == Status.New).CountAsync();
            return new ApplicantHistoryCountResult() {
                Success=true,
                ApplicantHistoryCount= new ApplicantHistoryCount() { Interview=interview,TotalApplied=totalApply,Waiting=waiting}
            };
        }

        public async Task<List<ApplicationHistoryData>> ApplicationHistoryData(string user)
        {
            var userCvData = await context.UserCvDatas.Where(x => x.UserId == user).Select(x=>x.Id).ToListAsync();
            var jobs = await context.UserJobs.Where(x => userCvData.Contains(x.userId)).Select(x => new
            ApplicationHistoryData()
            {
                ApplyDate = x.AppliedAt,
                JobId = x.jobID,
                JobName = x.job.Title,
                JobStatus = x.Status
            }).ToListAsync();
            return jobs;
            
        }
        public async Task<UserCount> UserCount(string user)
        {
            var userCvData = await context.UserCvDatas.Where(x => x.UserId == user)
                .Select(x=>x.Id)
                .ToListAsync();

            var userJobs = await context.UserJobs.Where(x => userCvData.Contains(x.userId)).ToListAsync();

            var applied = userJobs.Count();
            var interview = userJobs.Where(x => x.Status == Status.Interview).Count();
            var messages = await context.convesations
     .Where(x => userCvData.Contains(x.UserId))
     .Select(x => x.CompanyId)
     .Distinct()
     .CountAsync();
            return new UserCount()
            {
                Applied = applied,
                interview=interview,
                Messages=messages
            };
        }
    }
}
