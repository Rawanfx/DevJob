using AutoMapper;
using DevJob.Application.DTOs;
using DevJob.Application.DTOs.Company;
using DevJob.Application.DTOs.Cvs;
using DevJob.Application.DTOs.Jobs;
using DevJob.Domain.Entities;
using DevJob.Domain.Enums;
using DevJob.Application.Repository_Contract;
using DevJob.Application.ServiceContract;
using DevJob.Infrastructure.Hubs;
using Hangfire;
using HtmlAgilityPack;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Transactions;
namespace DevJob.Infrastructure.Service
{
    public class JobService : IJobServices
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration configuration;
        private readonly System.Net.Http.IHttpClientFactory httpClientFactory;
        private readonly INotificationService notificationService;
        private readonly ILogger<JobService> logger;
        private readonly IHubContext<JobHub> hubContext;
        private readonly IJobRepository jobRepository;
        private readonly IRecommendedJobRepository recommendedJobRepository;
        private readonly IUserSkillsRepository userSkillsRepository;
        private readonly IUserJobRepository userJobRepository;
        public JobService(IUnitOfWork unitOfWork, IMapper mapper,
            IConfiguration configuration
            , System.Net.Http.IHttpClientFactory httpClientFactory
            , INotificationService notificationService
            , ILogger<JobService> logger
            ,IHubContext<JobHub> hubContext
            ,IJobRepository jobRepository
            ,IRecommendedJobRepository recommendedJobRepository
            ,IUserSkillsRepository userSkillsRepository
            ,IUserJobRepository userJobRepository)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
            this.notificationService = notificationService;
            this.hubContext = hubContext;
            this.jobRepository = jobRepository;
            this.recommendedJobRepository = recommendedJobRepository;
            this.userSkillsRepository = userSkillsRepository;
            this.userJobRepository = userJobRepository;
        }
        private string CleanDescription(string description)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(description);
            var res = doc.DocumentNode.InnerText;
            return res;
        }
        public async Task<PostJobResult> AddJob(PostJobDTO postJobDTO, string company)
        {
            var companyId = await unitOfWork.CompanyProfile.FirstOrDefaultAsync(x => x.ApplicationUser == company);
            if (companyId == null)
                return new PostJobResult { Success = false, Message = "Invalid Company" };
            var row = (postJobDTO.Title + postJobDTO.Description + companyId.Id + postJobDTO.JobLevel)
                .ToLower().Replace(" ", "");
            string jobHash;
            using (var sh = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sh.ComputeHash(System.Text.Encoding.UTF8.GetBytes(row));
                jobHash = Convert.ToBase64String(bytes);

            }
            if (await unitOfWork.Jobs.AnyAsync(x => x.Hash == jobHash && x.IsActive))
            {
                return new PostJobResult() { Success = false, Message = "Duplicate job" };
            }
           await unitOfWork.BeginTransaction();
            try
            {

                Job job = _mapper.Map<Job>(postJobDTO);
                job.CreatedAt = DateTime.UtcNow;
                job.CompanyId = companyId.Id;
                job.Local = true;
                job.Hash = jobHash;
                await unitOfWork.Jobs.AddAsync(job);
                await unitOfWork.SaveChangesAsync();
                //add skills

                var comingSkills = postJobDTO.skills.Distinct().ToList()?? new List<string>();

                var existingSkills = await unitOfWork.Skills
                .Where(x => comingSkills.Contains(x.SkillName)).ToDictionaryAsync(x => x.SkillName, x => x.Id);

                var newSkills = comingSkills
            .Where(s => !existingSkills.ContainsKey(s))
            .Select(s => new Skills { SkillName = s })
            .ToList();
                if (newSkills.Any())
                {
                    await unitOfWork.Skills.AddRangeAsync(newSkills);
                    await unitOfWork.SaveChangesAsync();
                    foreach (var s in newSkills)
                        existingSkills[s.SkillName] = s.Id;
                }

                var requiredSkills = existingSkills.Values
            .Select(skillId => new RequiredSkills { JobId = job.Id, SkillId = skillId })
            .ToList();
                await unitOfWork.RequiredSkills.AddRangeAsync(requiredSkills);
                await unitOfWork.SaveChangesAsync();
                BackgroundJob.Enqueue<IBackgroundService>(x => x.CalculateMatchingForNewJob(job.Id));
                // BackgroundJob.Enqueue<IBackgroundService>(x => x.CalculateMatchJobsWithoutAi());
                await unitOfWork.CommitAsync();
                //signalR
                int totalActive = await unitOfWork.Jobs.Where(x => x.IsActive && x.CompanyId == companyId.Id).CountAsync();
                var data = new { 
                active = totalActive,
                compantId = companyId.ApplicationUser
                };
               await hubContext.Clients.User(company).SendAsync("ActiveJobs", data);
                return new PostJobResult { Job = postJobDTO, Success = true, Message = "Job added Successfully" };
            }
            catch (Exception ex)
            {
                await unitOfWork.RollBackAsync();
                throw;
            }
        }
        
        public async Task<ResultDto> Update(int jobId, string company, UpdateJobDto updateJobDto)
        {
            var companyId = await unitOfWork.CompanyProfile.FirstOrDefaultAsync(x => x.ApplicationUser == company);
            if (companyId == null)
                return new ResultDto() { Success = false, Message = "company not found" };
            var job = await unitOfWork.Jobs.FirstOrDefaultAsync(x => x.Id == jobId && companyId.Id == x.CompanyId && x.IsActive);
            if (job == null)
                return new ResultDto() { Success = false, Message = "job not found" };
            job.Description = updateJobDto.Desctiption;
            job.Location = updateJobDto.Location;
            job.DeadLine = updateJobDto.DeadLine;
            job.JobLevel = updateJobDto.JobLevel;
            job.EmploymentType = updateJobDto.EmploymentType;
            job.JobType = updateJobDto.JobType;

            await unitOfWork.SaveChangesAsync();
            return new ResultDto() { Success = true, Message = "updated" };
        }
        public async Task<List<AllJobsDto>> AllJobs(string company)
        {
            var companyId = await unitOfWork.CompanyProfile.FirstOrDefaultAsync(x => x.ApplicationUser == company);
            if (companyId == null)
                throw new KeyNotFoundException("Company Not Found");

            var jobCount = await jobRepository.GetCountOfApplicantForJob(companyId.Id);

            var jobs = await unitOfWork.Jobs.Where(x => x.CompanyId == companyId.Id && x.IsActive)
                .Select(x => new AllJobsDto()
                {
                    Id = x.Id,
                    DeadLine = x.DeadLine,
                    Desctiption = x.Description,
                    EmploymentType = x.EmploymentType,
                    JobLevel = x.JobLevel,
                    JobType = x.JobType,
                    Location = x.Location,
                    Title = x.Title,
                    Count = jobCount.GetValueOrDefault(x.Id, 0)
                }).ToListAsync();
            return jobs;
        }
        public async Task<ResponseDto> Delete(int jobId, string company)
        {
            var companyId = await unitOfWork.CompanyProfile.FirstOrDefaultAsync(x => x.ApplicationUser == company);
            if (companyId == null)
                return new ResponseDto() { Success = false, Message = "company not found" };
            var job = await unitOfWork.Jobs.FirstOrDefaultAsync(x => x.CompanyId == companyId.Id
            && x.IsActive && x.Id == jobId);
            if (job == null)
                return new ResponseDto() { Success = false, Message = "not found" };
            job.IsActive = false;
            await unitOfWork.SaveChangesAsync();
            return new ResponseDto() { Success = true, Message = "deleted" };
        }
        public async Task<ResultDto> GetJobsFromSerpApi(string userId)
        {
            // 1. Get User Context (CVs and Keywords)
            var cvs = await unitOfWork.Cvs
                .Where(x => x.UserId == userId)
                .Select(x => x.Id)
                .ToListAsync();

            List<string> qRequestsToSerpApi = await unitOfWork.SearchKeyWords
                .Where(x => cvs.Contains(x.cvId))
                .Select(x => x.Name)
                .ToListAsync();

            string apiKey = configuration["SerpApi"];
            var allFoundJobs = new List<JobsDto>();
            var processedJobIds = new HashSet<string>();

            using var httpClient = httpClientFactory.CreateClient();

            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");

            // 2. Loop Through Keywords
            foreach (var searchKeyWord in qRequestsToSerpApi)
            {
                string nextPageToken = null;
                int pageCount = 0;
                int maxPagesPerKeyword = 4;

                string encodedKeyword = System.Net.WebUtility.UrlEncode(searchKeyWord);

                do
                {
                    try
                    {
                        string url = $"https://serpapi.com/search.json?engine=google_jobs&q={encodedKeyword}&hl=en&api_key={apiKey}";

                        if (!string.IsNullOrEmpty(nextPageToken))
                        {
                            url += $"&next_page_token={nextPageToken}";
                        }

                        var response = await httpClient.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var jsonNode = JsonNode.Parse(jsonString);

                            if (jsonNode?["jobs_results"] is JsonNode jobsArray)
                            {
                                var currentBatch = JsonSerializer.Deserialize<List<JobsDto>>(jobsArray, new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                });

                                if (currentBatch != null)
                                {
                                    foreach (var job in currentBatch)
                                    {
                                        if (!string.IsNullOrEmpty(job.job_id) && processedJobIds.Add(job.job_id))
                                        {
                                            job.description = CleanDescription(job.description);
                                            allFoundJobs.Add(job);
                                        }
                                    }
                                }
                            }

                            nextPageToken = jsonNode?["serpapi_pagination"]?["next_page_token"]?.ToString();
                            pageCount++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {

                        continue;
                    }

                } while (!string.IsNullOrEmpty(nextPageToken) && pageCount < maxPagesPerKeyword);
            }

            return new ResultDto()
            {
                Success = true,
                jobsDtos = allFoundJobs,
                jobCount = allFoundJobs.Count
            };
        }
        public async Task<ResultDto> Apply(int jobId, string userId, int cvId)
        {
            var userCvData = await unitOfWork.UserCvData
                .FirstOrDefaultAsync(x => x.UserId == userId && x.cvId == cvId && !x.CV.IsDeleted);
            if (userCvData == null)
                return new ResultDto { Success = false, Message = "User CV not found" };

            var job = await jobRepository.GetActiveJob(jobId);
            if (job == null || job.Local==false)
                return new ResultDto { Success = false, Message = "Job not found" };

            if (await unitOfWork.UserJob.AnyAsync(x => x.jobID == jobId && x.userId == userCvData.Id))
                return new ResultDto { Success = false, Message = "Already applied" };


            var userJob = new UserJob
            {
                jobID = jobId,
                userId = userCvData.Id,
                AppliedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                Status = Status.New,
                cvId = cvId
            };

            await unitOfWork.UserJob.AddAsync(userJob);
            await unitOfWork.SaveChangesAsync();
            //get total applicants fro this  user
            var usersId = await unitOfWork.UserCvData.Where(x => x.UserId == userId)
                .Select(x => x.Id)
                .ToListAsync();
            var applicants = await unitOfWork.UserJob
                .Where(x => x.jobID == jobId)
               // .Select(x=>x.Status)
                .ToListAsync();
            var count = await unitOfWork.UserJob.Where(x => usersId.Contains(x.userId)).CountAsync();
            var applicantCount =applicants .Count();
            var newCountForComany = applicants.Count(x => x.Status == Status.New);
            //signalR
            await hubContext.Clients.User(userId).SendAsync("UpdateApplyCount",
                new{
                    user = userId,
                    ApplyCountForDeveloper = count,
                });
           await hubContext.Clients.User(job.companyProfile.ApplicationUser).SendAsync("UpdateApplyCount",
              new  {
               
                ApplyCountForCompany=applicantCount,
                newCountForCompany = newCountForComany
              });
            return new ResultDto { Success = true, Message = "Applied successfully" };
        }
        public async Task<List<RecommendedJobDto>> DisplayAllJobs()
        {
            var jobs = await unitOfWork.Jobs
                .Where(x=>x.IsActive)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new RecommendedJobDto()
                {
                    apply_Link=x.ApplyLink,
                    CompanyName=x.CompanyName,
                    DeadLine= x.DeadLine,
                    Desctiption=x.Description,
                    EmploymentType=x.EmploymentType,
                    JobLevel=x.JobLevel,
                    JobType=x.JobType,
                    Location=x.Location,
                    MatchScore=0,
                    PostedAt=x.PostedAt,
                    Title=x.Title,
                    job_id=x.Id
                }).Take(20)
                .ToListAsync();
            return jobs;
        }
        private async Task<DisplayRecommendedJobsDto> Helper(string userId)
        {
            var userCvData = await unitOfWork.UserCvData
                .Where(x => x.UserId == userId).Select(x => x.Id).ToListAsync();
            logger.LogInformation($"display recommended job with id {userId}");
            if (userCvData == null)
                return new DisplayRecommendedJobsDto() { Success = false, Message = "User not Found" };
            var recommendedJobs = await recommendedJobRepository.GetRecommendedJobForUser(userCvData);
            DisplayRecommendedJobsDto displayRecommendedJobsDto = new DisplayRecommendedJobsDto()
            {
                Success = true,
                RecommendedJobs = recommendedJobs
            };
            return displayRecommendedJobsDto;
        }
        public async Task<DisplayRecommendedJobsDto> DisplayRecommendedjobs2(string userId)
        {
            var userCvDataId = await unitOfWork.UserCvData.Where(x => x.UserId == userId).Select(x => x.Id).ToListAsync();
            var jobs = await unitOfWork.RecommendedJobs.Where(x => userCvDataId.Contains(x.userId))
                .Select(x => new RecommendedJobDto
                {
                    apply_Link = x.Job1.ApplyLink,
                    CompanyName = x.Job1.CompanyName,
                    Desctiption = x.Job1.Description,
                    EmploymentType = x.Job1.EmploymentType,
                    JobLevel = x.Job1.JobLevel,
                    Location = x.Job1.Location,
                    JobType = x.Job1.JobType,
                    MatchScore = x.MatchScore,
                    job_id = x.jobId,
                    PostedAt = x.Job1.PostedAt,
                    Title = x.Job1.Title,
                    DeadLine = x.Job1.DeadLine,
                })
                .ToListAsync();
            DisplayRecommendedJobsDto displayRecommendedJobsDto = new()
            {
                Success = true,
                RecommendedJobs = jobs
            };
            return displayRecommendedJobsDto;
        }
        public async Task<DisplayRecommendedJobsDto> DisplayRecommendedjobs(string userId)
        {
            var recommendedJobs = await Helper(userId);
            if (recommendedJobs.RecommendedJobs == null || recommendedJobs.RecommendedJobs.Count < 2)
            {
                var searchKey = await unitOfWork.SearchKeyWords.Where(x => x.IsActive).Select(x => x.Name).ToListAsync();
                BackgroundJob.Enqueue<IBackgroundService>(x => x.GetNewJobsFromSerpApi(searchKey));
                BackgroundJob.Enqueue<IBackgroundService>(x => x.CalculateMatchJobs());
            }
            return recommendedJobs;
        }
        public async Task<GetApplicantResultDto> GetApplicants(int jobId, string companyId)
        {
            var company = await unitOfWork.CompanyProfile.FirstOrDefaultAsync(x => x.ApplicationUser == companyId);
            if (company == null)
                return new GetApplicantResultDto() { Success = false, Message = "Company not found" };

            if (!await unitOfWork.Jobs.AnyAsync(x => x.Id == jobId && x.CompanyId == company.Id))
                return new GetApplicantResultDto() { Success = false, Message = "Job not found" };

            var scores = await jobRepository.GetScoreForApplicant(jobId);

            var userIds = await unitOfWork.UserJob.Where(x => x.jobID == jobId).Select(x => x.userId).ToHashSetAsync();
            var userSkills = await userSkillsRepository.GetUserSkills(userIds);

            var applicantsList = await userJobRepository.GetApplicantsForJob(jobId);

            var applicantsDto = applicantsList.Select(x => new GetApplicantDto()
            {
                userId = x.userId,
                ApplyDate = x.AppliedAt,
                MatchScore = scores.ContainsKey(x.userId) ? scores[x.userId] : 0,
                Name = x.UserCvData?.Name ?? "Unknown",
                skillName = userSkills.ContainsKey(x.userId) ? userSkills[x.userId] : new List<string>(),
                status = x.Status,
                YearOfex = (decimal)(x.UserCvData?.YearOfex ?? 0)
            })
            .OrderByDescending(d => d.MatchScore)
            .ToList();

            return new GetApplicantResultDto() { Success = true, getApplicantDtos = applicantsDto };

        }
        public async Task<UpdateStatusResultDto> UpdateStatus(UpdateStatusDto updateStatusDto, string companyId)
        {
            //check user found or not
            var user = await unitOfWork.UserCvData
                .FirstOrDefaultAsync(x => x.Id == updateStatusDto.UserId);
            if (user==null)
                return new UpdateStatusResultDto() { Success = false, Message = "user not found" };
            var company = await unitOfWork.CompanyProfile.FirstOrDefaultAsync(x => x.ApplicationUser == companyId);
            if (company == null)
                return new UpdateStatusResultDto() { Success = false, Message = "Company not found" };

            if (!await unitOfWork.Jobs.AnyAsync(x => x.Id == updateStatusDto.JobId && x.CompanyId == company.Id))
                return new UpdateStatusResultDto() { Success = false, Message = "Job not found" };
            var userjob = await unitOfWork.UserJob.FirstOrDefaultAsync(x => x.jobID == updateStatusDto.JobId && x.userId == updateStatusDto.UserId);
            if (userjob == null)
                return new UpdateStatusResultDto() { Success = false, Message = "user didn't apply" };
            if (userjob.Status == updateStatusDto.status)
                return new UpdateStatusResultDto() { Success = false, Message = "Status hasn't been changed" };
           await unitOfWork.BeginTransaction();
            userjob.Status = updateStatusDto.status;

            string title = "";
            string message = "";

            switch (updateStatusDto.status)
            {
                case Status.Interview:
                    title = "Interview Scheduled! ";
                    message = "You have been selected for an interview. Please check your dashboard for more info.";
                    break;
                case Status.Rejected:
                    title = "Application Update";
                    message = "We appreciate your interest. Unfortunately, you were not selected for this position.";
                    break;
                case Status.Reviewed:
                    title = "Application Reviewed";
                    message = "Your profile has been reviewed and is currently under consideration.";
                    break;
                case Status.Accepted:
                    title = "Application Accapted";
                    message = "Congratulations! Your application has been accepted.";
                    break;
            }
            try
            {
                await unitOfWork.SaveChangesAsync();
                if (updateStatusDto.status != Status.New)
                {
                    var res = await notificationService.SendNotification(user.UserId, title, message);
                    var saveNotification = await notificationService.SaveNotification(updateStatusDto.UserId, message, title);
                    if (!res.Success || !saveNotification)
                        throw new Exception(res.Message);
                }
                await unitOfWork.CommitAsync();
                //signalR
             
                var allApplicants = await unitOfWork.UserJob
                    .Where(x => x.jobID == updateStatusDto.JobId)
                    .ToListAsync();

                var allStatusForUser = await unitOfWork.UserJob
                    .Where(x => x.userId == updateStatusDto.UserId)
                    .ToListAsync();
                var companyData = new
                {
                    newStatus = updateStatusDto.status.ToString(),
                    countInterviewForCompany = allApplicants.Count(x => x.Status == Status.Interview),
                    countAcceptedForCompany = allApplicants.Count(x => x.Status == Status.Accepted),
                    countRejectedForCompany = allApplicants.Count(x => x.Status == Status.Rejected),
                    countNewForCompany = allApplicants.Count(x => x.Status == Status.New),
                };
                var developerData =new {
                    newStatus = updateStatusDto.status.ToString(),
                    countAcceptedForDeveloper = allStatusForUser.Count(x => x.Status == Status.Accepted),
                    countRejectedForDeveloper = allStatusForUser.Count(x => x.Status == Status.Rejected),
                    countNewForDeveloper = allStatusForUser.Count(x => x.Status == Status.New),
                    countInterviewForDeveloper = allStatusForUser.Count(x => x.Status == Status.Interview),
                };
                await hubContext.Clients.User(companyId).SendAsync("UpdateStatus", companyData);
                await hubContext.Clients.User(user.UserId).SendAsync("UpdateStatus", developerData);
                return new UpdateStatusResultDto() { Success = true, Message = "updated" };
            }
            catch (Exception ex)
            {
                await unitOfWork.RollBackAsync();
                throw;
            }

        }
        public async Task<List<string>> GetAllSkills()
        {
            var res = await unitOfWork.Skills
                .GetAllAsyncASQuarable()
                .Select(x => x.SkillName).ToListAsync();
            return res;
        }
        public async Task<ApplicantsCountResult> ApplicantsCount(string company, int jobId)
        {
            var companyId = await unitOfWork.CompanyProfile.FirstOrDefaultAsync(x => x.ApplicationUser == company);
            if (companyId == null)
                return new ApplicantsCountResult() { Success = false, Message = "campany not found" };
            if (!await unitOfWork.Jobs.AnyAsync(x => x.Id == jobId && x.CompanyId == companyId.Id))
                return new ApplicantsCountResult() { Success = false, Message = "job not found" };
            var totalApplicants = await unitOfWork.UserJob.Where(x => x.jobID == jobId).CountAsync();
            var tatalReviewed = await unitOfWork.UserJob.Where(x => x.jobID == jobId && x.Status ==Status. Reviewed).CountAsync();
            var tatalRejected = await unitOfWork.UserJob.Where(x => x.jobID == jobId && x.Status ==Status .Rejected).CountAsync();
            var tatalNew = await unitOfWork.UserJob.Where(x => x.jobID == jobId && x.Status == Status.New).CountAsync();
            var tatalInterview = await unitOfWork.UserJob.Where(x => x.jobID == jobId && x.Status == Status.Interview).CountAsync();
            return new ApplicantsCountResult()
            {
                applicantsCount = new ApplicantsCount() { TotalApplicant = totalApplicants,
                    TotalInterview = tatalInterview,
                    TotalNew = tatalNew,
                    TotalReviewed = tatalReviewed
                },
                Success = true
            };
        }
        public async Task<UserPrefereResultDto> AddUserPrefare(UserPrefareDto userPrefareDto, string user)
        {
            var userCvData = await unitOfWork.UserCvData.FirstOrDefaultAsync(x => x.UserId == user);
            if (userCvData == null)
                return new UserPrefereResultDto() { Success = false, Message = "User not found" };
            if (userPrefareDto.jobTypes != null)
            {
                List<UserPreferenceـJobs> UserPreferenceـJobs = new();
                foreach (var i in userPrefareDto.jobTypes)
                {
                    UserPreferenceـJobs.Add(new UserPreferenceـJobs()
                    {
                        JobType = i,
                        userId = userCvData.Id
                    });
                }
                await unitOfWork
                    .UserPreferenceـJobs
                    .AddRangeAsync(UserPreferenceـJobs);
            }
            if (userPrefareDto.skills != null)
            {
                List<UserPrefernce_Skills> skills = new();
                //get all skillsId
                var skillsId = await unitOfWork.Skills.Where(x => userPrefareDto.skills.Contains(x.SkillName))
                    .Select(x => new { x.Id, x.SkillName })
                    .ToDictionaryAsync(x => x.SkillName, x => x.Id);
                foreach (var i in userPrefareDto.skills)
                {
                    skills.Add(new UserPrefernce_Skills()
                    {
                        SkillId = skillsId[i],
                        userCvData = userCvData.Id,
                    });
                }
                await unitOfWork.UserPrefernce_Skills.AddRangeAsync(skills);
            }
            UserPreference userPreference = new UserPreference()
            {
                userId = userCvData.Id,
                JobLevel = userPrefareDto.JobLevel,
                MinimumSalary = userPrefareDto.MinimumSalar,
            };
            await unitOfWork.UserPreference.AddAsync(userPreference);
            await unitOfWork.SaveChangesAsync();
            return new UserPrefereResultDto() { Success = true };
        }
        public async Task<ComapnyCountResult> CompanyCount(string user)
        {
            var company = await unitOfWork.CompanyProfile.FirstOrDefaultAsync(x => x.ApplicationUser == user);
            if (company == null)
                return new ComapnyCountResult()
                {
                    Success = false,
                    Message = "Company not found"
                };

            var jobs = await unitOfWork.Jobs
                .Where(x => x.CompanyId == company.Id)
                .ToDictionaryAsync(x => x.Id, x => x.IsActive);

            var jobIds = jobs.Keys.ToList();
            var activeJobs = jobs.Values.ToList();
            var allUsers = await unitOfWork.UserJob.Where(x => jobIds.Contains(x.jobID)).ToListAsync();
            var totalApplicant = allUsers.Count();
            var hires = allUsers.Where(x => x.Status == Status.Accepted).Count();
            var activeJob = jobs.Where(x => activeJobs.Contains(true)).Count();
            return new ComapnyCountResult()
            {
                Success = true,
                CompanyCount = new()
                {
                    ActiveJob = activeJob,
                    Applicants = totalApplicant,
                    Hires = hires
                }
            };
        }
        public async Task<AddSavedJobsResult> AddSavedJobs(AddSavedJobsDto addSavedJobsDto, string appUser)
        {
            var user = await unitOfWork.UserCvData.FirstOrDefaultAsync(x => x.Id == addSavedJobsDto.userId
            && x.UserId == appUser);
            if (user == null)
                return new AddSavedJobsResult() { Success = false, Message = "User not found" };
            var job = await unitOfWork.Jobs.FirstOrDefaultAsync(X => X.Id == addSavedJobsDto.jobId &&X.IsActive);
            if (job == null)
                return new AddSavedJobsResult() { Success = false, Message = "Job not found" };

            SavedJobs savedJobs = new SavedJobs()
            {
                jobId = addSavedJobsDto.jobId,
                date = DateOnly.FromDateTime(DateTime.Now),
                userId = addSavedJobsDto.userId
            };
            var isFound = await unitOfWork.SavedJobs.FirstOrDefaultAsync(x => 
             x.jobId == savedJobs.jobId
            &&x.userId==savedJobs.userId
            );
            if (isFound!=null)
            {
                unitOfWork.SavedJobs.Remove(isFound);
                await unitOfWork.SaveChangesAsync();
                return new AddSavedJobsResult() { Success = true, Message = "job has removed from saved" };
            }
            await unitOfWork.SavedJobs.AddAsync(savedJobs);
            await unitOfWork.SaveChangesAsync();
            return new AddSavedJobsResult() { Success = true, Message = "Added" };
        }
        public async Task<DisplaySavedJobDtoResult> DisplaySavedJob(string appUser)
        {
            var user = await unitOfWork.UserCvData.Where(x=> x.UserId==appUser)
                .Select(x=>x.Id)
                .ToListAsync();
            if (user == null)
                return new DisplaySavedJobDtoResult() { Success = false, Message = "user not found" };

            var requiredSkills = await jobRepository.GetRequiredSkills();

            var savedJobs = await unitOfWork.SavedJobs
                 .Where(x=>user.Contains(x.userId))
                 .Select(x => new DisplaySavedJobDto()
                 {
                     jobId=x.jobId,
                 jobName = x.Job1.Title,
                 Location = x.Job1.Location,
                 SavedDate = x.date,
                 Skills = requiredSkills.ContainsKey(x.jobId) ? requiredSkills[x.jobId]:null,
                 IsExternal=!x.Job1.Local,
                 ApplyLink=x.Job1.ApplyLink
             })
                 .Distinct()
                 .ToListAsync();
            return new DisplaySavedJobDtoResult()
            {
                Success = true,
                displaySavedJobDtos = savedJobs
            };
        }

        public async Task<List<RecommendedJobDto>> JobSearch(string item)
        {
            var result = await jobRepository.JobSearch(item);
            return result;
        }
        public async Task<DisplaySavedJobDtoResult> SavedJobSearch(string item, string user)
        {
            var userIds = await unitOfWork.UserCvData.Where(x => x.UserId == user)
                .Select(x => x.Id)
                .ToListAsync();
            if (userIds.Count == 0)
                return new DisplaySavedJobDtoResult() { Success = false, Message = "User Not Found" };
            var res = await jobRepository.SavedJobSearch(item, userIds);
            return new DisplaySavedJobDtoResult() { Success = true, displaySavedJobDtos = res };
        }
    }
}