
using DevJob.Application.DTOs.Cvs;
using DevJob.Application.DTOs.InputDataFromJson;
using DevJob.Domain.Entities;
using DevJob.Application.ServiceContract;
using DevJob.Infrastructure.Data;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DevJob.Infrastructure.Service
{
    public class CvServices : ICVServices
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IUnitOfWork unitOfWork;
        private readonly IUploadToAzure uploadToAzure;
        private readonly IConfiguration configuration;
        private readonly ILogger<CvServices> logger;
        private readonly UserManager<ApplicationUser> userManager;
        public CvServices(
            IUnitOfWork unitOfWork,
            IHttpClientFactory httpClientFactory,
            IUploadToAzure uploadToAzure,
            IConfiguration configuration
            ,UserManager<ApplicationUser> userManager
            ,ILogger<CvServices>logger)
        {
            this.unitOfWork = unitOfWork;
            this.httpClientFactory = httpClientFactory;
            this.uploadToAzure = uploadToAzure;
            this.userManager = userManager;
            this.configuration = configuration;
            this.logger = logger;
        }

        public async Task<List<AllCvsDto>> AllCv(string userId)
        {
            return await unitOfWork.Cvs
               .Where(x => x.UserId == userId && x.IsDeleted==false)
                .Select(x => new AllCvsDto { CvName = x.FileName, Id = x.Id })
                .ToListAsync();
        }

        public async Task<ResponseDto> Upload(string userId, IFormFile file)
        {
            string fileHash;
            using (var sha256 = SHA256.Create())
            using (var stream = file.OpenReadStream())
            {
                fileHash = BitConverter.ToString(sha256.ComputeHash(stream))
                    .Replace("-", "").ToLower();
            }

            if (await unitOfWork.Cvs.AnyAsync(x => x.UserId == userId && x.Hash == fileHash && !x.IsDeleted))
                return new ResponseDto { Success = false, Message = "This CV has already been uploaded." };

            var userProfile = await unitOfWork.UserProfile.FirstOrDefaultAsync(x => x.userId == userId);
            if (userProfile == null) return new ResponseDto { Success = false, Message = "User profile not found." };

         
            UploadToAzureResult upload = null;
            CV cv = null;

            try
            {
              
                upload = await uploadToAzure.UploadFileToAzure(file, userId);

                cv = new CV
                {
                    FileName = file.FileName,
                    Hash = fileHash,
                    FileNameAzure = upload.FileNameAzure,
                    Path = upload.Link,
                    UserId = userProfile.userId,
                    UserProfile = userProfile
                };

                await unitOfWork.Cvs.AddAsync(cv);
                await unitOfWork.SaveChangesAsync();

             
                var parseResult = await ParceCv(configuration["url"], file, userId, cv.Id);

                if (!parseResult.Success)
                {
                  
                    throw new Exception("Parsing failed: " + parseResult.Message);
                }

           
                BackgroundJob.Enqueue<IBackgroundService>(x => x.PrepareRecommendedJobs(userId, cv.Id));

                return new ResponseDto { Success = true };
            }
            catch (Exception ex)
            {
              
                if (upload != null) await uploadToAzure.Delete(upload.FileNameAzure);

                if (cv != null && cv.Id > 0)
                {
                    var existingCv = await unitOfWork.Cvs.FirstOrDefaultAsync(x=>x.Id==cv.Id);
                    if (existingCv != null)
                    {
                        unitOfWork.Cvs.Remove(existingCv);
                        await unitOfWork.SaveChangesAsync();
                    }
                }

                throw;
            }
        }
        public async Task<parseResult> ParceCv(string url, IFormFile file, string userID, int cvId)
        {
         
            var user = await userManager.FindByIdAsync(userID);
            if (user == null) throw new KeyNotFoundException("User not found");

            
            var client = httpClientFactory.CreateClient();
            using var form = new MultipartFormDataContent();
            using var stream = file.OpenReadStream();
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            form.Add(streamContent, "file", file.FileName);

            var response = await client.PostAsync(url, form);
            if (!response.IsSuccessStatusCode)
                return new parseResult { Success = false, Message = "AI Service Error" };

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<AllDataInput>(json);
            if (data == null)
                return new parseResult { Success = false, Message = "Empty AI Data" };

            var incomingSkills = data.skills?.Distinct().ToList() ?? new List<string>();

            await unitOfWork.BeginTransaction();
            try
            {
                var userCvData = new UserCvData
                {
                    Name = data.name,
                    Email = data.email,
                    LinkedInAccount = data.linkedin,
                    JobTitle = data.job_title,
                    UserId = userID,
                    cvId = cvId,
                };
                await unitOfWork.UserCvData.AddAsync(userCvData);
                await unitOfWork.SaveChangesAsync(); 

                var existingKeywords = await unitOfWork.SearchKeyWords
                    .GetAllAsyncASQuarable()
                    .Select(x => x.Name).ToHashSetAsync();
                var searchToAdd = new SearchKeyWord() { Name = data.predicted_job_role, cvId = cvId };
               await unitOfWork.SearchKeyWords.AddAsync(searchToAdd);
            
                var existingSkillsInDb = await unitOfWork.Skills
                    .Where(s => incomingSkills.Contains(s.SkillName))
                    .ToListAsync();

               
                var skillsMap = existingSkillsInDb
                    .GroupBy(s => s.SkillName)
                    .ToDictionary(g => g.Key, g => g.First().Id);

                var skillsToAdd = incomingSkills
                    .Where(s => !skillsMap.ContainsKey(s))
                    .Select(s => new Skills { cvId = cvId, SkillName = s })
                    .ToList();

                if (skillsToAdd.Any())
                {
                    await unitOfWork.Skills.AddRangeAsync(skillsToAdd);
                    await unitOfWork.SaveChangesAsync();

                    foreach (var s in skillsToAdd)
                        skillsMap[s.SkillName] = s.Id;
                }

                var userSkillsToAdd = skillsMap.Values.Select(skillId => new UserSkills
                {
                    cvId = cvId,
                    SkillId = skillId,
                    UserId = userCvData.Id 
                }).ToList();

                await unitOfWork.UserSkills.AddRangeAsync(userSkillsToAdd);
       
                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();

                return new parseResult { Success = true, UsercvData = userCvData.Id };
            }
            catch (Exception ex)
            {
                await unitOfWork.RollBackAsync();
                logger.LogError(ex,"Error occured in ParseCv {userId}", userID);
                throw;
              
            }
        }
        //public async Task<UploadCvResult> Upload2(string userId, IFormFile file)
        //{

        //    string fileHash;
        //    using (var sha256 = SHA256.Create())
        //    using (var stream = file.OpenReadStream())
        //    {
        //        fileHash = BitConverter.ToString(sha256.ComputeHash(stream))
        //            .Replace("-", "").ToLower();
        //    }

        //    if (await context.cVs.AnyAsync(x => x.UserId == userId && x.Hash == fileHash))
        //        return new UploadCvResult { Success = false, Message = "Duplicate CV" };

        //    var userProfile = await context.UserProfile
        //        .FirstOrDefaultAsync(x => x.userId == userId);

        //    var upload = await uploadToAzure.UploadFileToAzure(file, userId);
        //    try
        //    {
        //        var cv = new CV
        //        {
        //            FileName = file.FileName,
        //            Hash = fileHash,
        //            FileNameAzure = upload.FileNameAzure,
        //            Path = upload.Link,
        //            UserId = userProfile.userId,
        //            UserProfile = userProfile
        //        };
        //        await context.cVs.AddAsync(cv);
        //        await context.SaveChangesAsync();

        //        UserCvData userCvData = new UserCvData()
        //        {
        //            cvId = cv.Id,
        //            Email = "demo",
        //            LinkedInAccount = "demo",
        //            Name = "demo",
        //            Summary = "demo",
        //            UserId = userId,
        //            gitub = "demo"
        //        };
        //        await context.userCvDatas.AddAsync(userCvData);
        //        await context.SaveChangesAsync();
        //        BackgroundJob.Enqueue<IBackgroundService>(x => x.PrepareWithoutAi(userCvData.Id, cv.Id));
        //        return new UploadCvResult() { Success = true };
        //    }
        //    catch(Exception ex)
        //    {
        //        return new UploadCvResult() { Success = false, Message = ex.Message };
        //    }
        //}

        public async Task<DeleteCvResult> DeleteCv(string userId, int cvId)
        {
            var cv = await unitOfWork.Cvs.FirstOrDefaultAsync(x => x.UserId == userId
            && x.Id == cvId &&!x.IsDeleted);

            if (cv != null)
            { 
                    var seachKey =await  unitOfWork.SearchKeyWords.Where(x => x.cvId == cvId).ToListAsync();
                    foreach (var i in seachKey)
                        i.IsActive = false;

                    //    await context.searchKeyWords.Where(x => x.cvId == cvId).ExecuteDeleteAsync();
                    //    await context.experiences.Where(x => x.cvId == cvId).ExecuteDeleteAsync();
                    //    await context.educations.Where(x => x.cvId == cvId).ExecuteDeleteAsync();
                    //    await context.userSkills.Where(x => x.cvId == cvId).ExecuteDeleteAsync();
                    //    await context.trainnings.Where(x => x.cvId == cvId).ExecuteDeleteAsync();
                    //    await context.ProjectSkills.Where(x => x.cvId == cvId).ExecuteDeleteAsync();
                    //    await context.Projects.Where(x => x.cvId == cvId).ExecuteDeleteAsync();
                    //    await context.userCvDatas.Where(x => x.cvId == cvId).ExecuteDeleteAsync();
                    //    //  await context
                    //    await uploadToAzure.Delete(cv.FileNameAzure);
                    //    context.cVs.Remove(cv);
                    //    await context.SaveChangesAsync();
                    //    await transaction.CommitAsync();
                    cv.IsDeleted = true;
                    unitOfWork.Cvs.Update(cv);
                   await unitOfWork.SaveChangesAsync();

                    return new DeleteCvResult() { Success = true, Message = "Deleted" };
               
            }
            else
                return new DeleteCvResult { Success = false, Message = "CV not found" };
        }
        }
    }

