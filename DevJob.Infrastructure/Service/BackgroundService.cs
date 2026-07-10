using DevJob.Application.DTOs;
using DevJob.Application.DTOs.AiRequest;
using DevJob.Application.DTOs.Jobs;
using DevJob.Domain.Entities;
using DevJob.Application.Repository_Contract;
using DevJob.Application.ServiceContract;
using DevJob.Infrastructure.Data;
using DevJob.Infrastructure.Repositories;
using Hangfire;
using Hangfire.Storage.Monitoring;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Serilog;


namespace DevJob.Infrastructure.Service
{
    public class BackgroundService : IBackgroundService
    {
        private readonly IConfiguration configuration;
        private readonly IUnitOfWork unitOfWork;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IMailServices mailServices;
        public BackgroundService(IUnitOfWork unitOfWork, IConfiguration configuration
            , IHttpClientFactory httpClientFactory
            , IMailServices mailServices
            )
        {
            this.unitOfWork=unitOfWork;
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory;
            this.mailServices = mailServices;
        }
        private string InitialSanitize(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";

            // 1. فك رموز الـ HTML (زي &amp; و &quot;)
            string decoded = HttpUtility.HtmlDecode(text);

            // 2. مسح الـ HTML Tags تماماً (أي حاجة بين < >)
            string noHtml = Regex.Replace(decoded, "<.*?>", string.Empty);

            string pattern = @"[^a-zA-Z0-9\u0600-\u06FF\s,.\-\(\):!&]";
            string noSpecialChars = Regex.Replace(noHtml, pattern, string.Empty);

        
            string finalText = Regex.Replace(noSpecialChars, @"\s+", " ").Trim();

            return finalText;
        }
        public async Task<List<Job>> GetNewJobsFromSerpApi(List<string> searchKey)
        {
            //get seach query
            //  var searchKey = await context.searchKeyWords.Where(x => x.IsActive).Select(x => x.Name).ToListAsync();
            //get all jobs
            var jobsInDb = await unitOfWork.Jobs.Where(x => !x.Local).ToListAsync();
            var Id = await unitOfWork.Jobs.Where(x => !x.Local).Select(x => x.JobIdExtension).ToHashSetAsync();
            string apiKey = configuration["SerpApi"];
            var allFoundJobs = new List<Job>();
            using (var httpClient = httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");
                // 2. Loop Through Keywords
                foreach (var searchKeyWord in searchKey)
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

                                            var row = (job.title + job.description + job.company_name + job.job_id)
                                             .ToLower().Replace(" ", "");
                                            string jobHash;
                                            using (var sh = System.Security.Cryptography.SHA256.Create())
                                            {
                                                byte[] bytes = sh.ComputeHash(System.Text.Encoding.UTF8.GetBytes(row));
                                                jobHash = Convert.ToBase64String(bytes);

                                            }
                                            if (!string.IsNullOrEmpty(job.job_id) && !Id.Contains(job.job_id) && !jobsInDb.Any(x => x.Hash == jobHash))
                                            {
                                                Job newJob = new Job()
                                                {
                                                    JobIdExtension = job.job_id,
                                                    ApplyLink = job.apply_options[0].link,
                                                    Source = job.apply_options[0].title,
                                                    CompanyName = job.company_name,
                                                    PostedAt = job.detected_extensions?.posted_at,
                                                    CreatedAt = DateTime.UtcNow,
                                                    Description = InitialSanitize(job.description),
                                                    IsActive = true,
                                                    Location = job.location,
                                                    Title = job.title,
                                                    Local = false,
                                                    IsProcessed = false,
                                                    Hash = jobHash
                                                };
                                                allFoundJobs.Add(newJob);

                                            }
                                            else
                                            {
                                                //found in database => update 
                                                var foundJob = jobsInDb.FirstOrDefault(x => x.JobIdExtension == job.job_id);
                                                if (foundJob == null)
                                                    continue;
                                                foundJob.ApplyLink = job.apply_options[0].link;
                                                foundJob.CompanyName = job.company_name;
                                                foundJob.Description = InitialSanitize(job.description);
                                                foundJob.Source = job.apply_options[0].title;
                                                foundJob.PostedAt = job?.detected_extensions?.posted_at;
                                                foundJob.Location = job.location;
                                                foundJob.Title = job.title;
                                                foundJob.Hash = jobHash;

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
                //update database
                //send to ai
                await unitOfWork.Jobs.AddRangeAsync(allFoundJobs);
                await unitOfWork.SaveChangesAsync();
                return allFoundJobs;
            }

        }

        public async Task<List<Job>> GetNewJobsFromSerpApi()
        {
            //get seach query
            var searchKey = await unitOfWork.SearchKeyWords.Where(x => x.IsActive).Select(x => x.Name).ToListAsync();
            //get all jobs
            var jobsInDb = await unitOfWork.Jobs.Where(x => !x.Local).ToListAsync();
            var Id = await unitOfWork.Jobs.Where(x => !x.Local).Select(x => x.JobIdExtension).ToHashSetAsync();
            string apiKey = configuration["SerpApi"];
            var allFoundJobs = new List<Job>();
            var httpClient = httpClientFactory.CreateClient();
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");
                // 2. Loop Through Keywords
                foreach (var searchKeyWord in searchKey)
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

                                            var row = (job.title + job.description + job.company_name + job.job_id)
                                             .ToLower().Replace(" ", "");
                                            string jobHash;
                                            using (var sh = System.Security.Cryptography.SHA256.Create())
                                            {
                                                byte[] bytes = sh.ComputeHash(System.Text.Encoding.UTF8.GetBytes(row));
                                                jobHash = Convert.ToBase64String(bytes);

                                            }
                                            if (!string.IsNullOrEmpty(job.job_id) && !Id.Contains(job.job_id) && !jobsInDb.Any(x => x.Hash == jobHash))
                                            {
                                                Job newJob = new Job()
                                                {
                                                    JobIdExtension = job.job_id,
                                                    ApplyLink = job.apply_options[0].link,
                                                    Source = job.apply_options[0].title,
                                                    CompanyName = job.company_name,
                                                    PostedAt = job.detected_extensions?.posted_at,
                                                    CreatedAt = DateTime.Now,
                                                    Description = InitialSanitize(job.description),
                                                    IsActive = true,
                                                    Location = job.location,
                                                    Title = job.title,
                                                    Local = false,
                                                    IsProcessed = false,
                                                    Hash = jobHash
                                                };
                                                allFoundJobs.Add(newJob);

                                            }
                                            else
                                            {
                                                //found in database => update 
                                                var foundJob = jobsInDb.FirstOrDefault(x => x.JobIdExtension == job.job_id);
                                                if (foundJob == null)
                                                    continue;
                                                foundJob.ApplyLink = job.apply_options[0].link;
                                                foundJob.CompanyName = job.company_name;
                                                foundJob.Description = InitialSanitize(job.description);
                                                foundJob.Source = job.apply_options[0].title;
                                                foundJob.PostedAt = job.detected_extensions.posted_at;
                                                foundJob.Location = job.location;
                                                foundJob.Title = job.title;
                                                foundJob.Hash = jobHash;

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
                //update database
                //send to ai
                await unitOfWork.Jobs.AddRangeAsync(allFoundJobs);
                await unitOfWork.SaveChangesAsync();
                return allFoundJobs;
            }

        }

        public async Task CalculateMatchingForNewJob(int jobId)
        {
            var job = await unitOfWork.Jobs.FirstOrDefaultAsync(x=>x.Id==jobId);
            if (job == null)
                return;
            var jobSkills = await unitOfWork.Jobs .GetJobSkills(new List<int>() { jobId });
            var currentJobSkills = string.Join(',', jobSkills);
            var jobData = $"Description: {job.Description}. Required Skills: {currentJobSkills}";
            //get allusers

            var rowData = await unitOfWork.UserCvData .GetUserData();
          
            var allUsers = rowData.GroupBy(x => x.userId)
                .Select(x => new AiUserRequest()
                {
                    user_id = x.Key,
                    cv_name = x.First().cvName,
                    cv_text = $"job_title: {x.First().jobTitle}, Skills: {string.Join(", ", x.Select(y => y.skills).Distinct())}"  
                }).ToList();

            if (!allUsers.Any()) return;
            var url = configuration["matchScoreUrl"];
            using var httpClient = httpClientFactory.CreateClient();
            List<RecommendedJobs> recommendedJobsToSave = new();
            var recommendeInDb = await unitOfWork.RecommendedJobs .RecommendedJobs();
            var requestBody = new AiMatcheRequest()
            {
                cvs = allUsers,
                job_description = jobData
            };
         

            try
            {
                var response = await httpClient.PostAsJsonAsync(url, requestBody);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<AiResponseDto>();

                    if (data?.results != null)
                    {
                        foreach (var match in data.results)
                        {
                            if (match.match_percentage >= 65 && !recommendeInDb.Contains($"{job.Id}_{match.user_id}"))
                            {

                                recommendedJobsToSave.Add(new RecommendedJobs()
                                {
                                    jobId = job.Id,
                                    MatchScore = match.match_percentage,
                                    userId = match.user_id
                                });
                                //string emailBody = $@"<h3>Dear {userInfo.Name}, AI found a match for {jobDetails.Title}...</h3>";
                                // BackgroundJob.Enqueue<IMailServices>(x => x.SendEmailAsync(userInfo.Email, "DevJob - New Match!", null, emailBody));
                            }
                        }
                    }
                }
                if (recommendedJobsToSave.Any())
                {
                    await unitOfWork.RecommendedJobs.AddRangeAsync(recommendedJobsToSave);
                }
                job.IsProcessed = true;
                await unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error processing chunk for user  {ex.Message}");
            }
        }

        public async Task CalculateMatchJobs()
        {
            var rowData = await unitOfWork.UserCvData .GetUserData();

            var allUsers = rowData.GroupBy(x => x.userId)
                .Select(x => new AiUserRequest()
                {
                    user_id=x.Key,
                    cv_name=x.First().cvName,
                    cv_text = $"job_title: {x.First().jobTitle}, Skills: {string.Join(", ", x.Select(y => y.skills).Distinct())}"
                }).ToList();

            if (!allUsers.Any()) return;

            var jobs = await unitOfWork.Jobs.Where(x =>!x.IsProcessed&&  x.IsActive)
                .ToListAsync();
            var jobIds = jobs.Select(x => x.Id).ToList();
            List<JobWithSkillsDto> jobSkills =await unitOfWork.Jobs .GetJobSkills(jobIds);

            if (!jobs.Any()) return;

            var url = configuration["matchScoreUrl"];
            using var httpClient = httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromMinutes(5);

            List<RecommendedJobs> recommendedJobsToSave = new();
            var recommendedInDb = await unitOfWork.RecommendedJobs
                          .GetAllAsyncASQuarable()
                         .Select(x => $"{x.jobId}_{x.userId}")
                        .ToHashSetAsync();

            foreach (var job in jobs)
            {
                var currentJobSkills = string.Join(", ", jobSkills.Where(x => x.JobId == job.Id).Select(x => x.Skills));
                var jobData = $"Description: {job.Description}. Required Skills: {currentJobSkills}";
                var requestBody = new AiMatcheRequest()
                {
                    cvs = allUsers,
                    job_description = jobData
                };

                try
                {
                    var response = await httpClient.PostAsJsonAsync(url, requestBody);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadFromJsonAsync<AiResponseDto>();

                        if (data?.results != null)
                        {
                            foreach (var match in data.results)
                            {
                                if (match.match_percentage >= 65 && !recommendedInDb.Contains($"{job.Id}_{match.user_id}"))
                                {
                                  
                                    recommendedJobsToSave.Add(new RecommendedJobs()
                                    {
                                        jobId = job.Id,
                                        MatchScore = match.match_percentage,
                                        userId =match.user_id
                                    });
                                    //string emailBody = $@"<h3>Dear {userInfo.Name}, AI found a match for {jobDetails.Title}...</h3>";
                                    // BackgroundJob.Enqueue<IMailServices>(x => x.SendEmailAsync(userInfo.Email, "DevJob - New Match!", null, emailBody));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"Error processing chunk for user  {ex.Message}");
                }

            }

            if (recommendedJobsToSave.Any())
            {
                await unitOfWork.RecommendedJobs.AddRangeAsync(recommendedJobsToSave);
            }

            foreach (var job in jobs)
            {
                job.IsProcessed = true;
            }

            await unitOfWork.SaveChangesAsync();
        }

        public async Task PrepareRecommendedJobs(string user, int cvId)
        {
            const int BatchSize = 200;
            const int MatchThreshold = 65;

            var userdata = await unitOfWork.UserCvData
                .Where(x => x.UserId == user
                         && x.cvId == cvId
                         && x.CV.IsDeleted == false)
                .Select(x => new { x.Id, x.JobTitle })
                .FirstOrDefaultAsync();

            if (userdata == null)
                return;

            var userId = userdata.Id;

            var skill = await unitOfWork.UserSkills.GetUserSkills(cvId, userId);
            var skillsCsv = string.Join(",", skill);

            var matchUrl = configuration["prepareMatchUrl"];
            var recommendedJobs = new List<RecommendedJobs>();

            using var httpClient = httpClientFactory.CreateClient();

            int lastJobId = 0;

            while (true)
            {
                var batch = await unitOfWork.Jobs
                    .Where(x => x.IsActive && x.Id > lastJobId)
                    .OrderBy(x => x.Id)
                    .Take(BatchSize)
                    .Select(x => new RequestJobs { id = x.Id, description = x.Description })
                    .ToListAsync();

                if (batch.Count == 0)
                    break;

                lastJobId = batch.Max(x => x.id);

                var aiRequest = new PrepareRecommendedRequest
                {
                    cv_skills = skillsCsv,
                    cv_title = userdata.JobTitle,
                    jobs = batch
                };

                HttpResponseMessage response;
                try
                {
                    response = await httpClient.PostAsJsonAsync(matchUrl, aiRequest);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);

                    continue; 
                }

                if (!response.IsSuccesatusCode)
                {
                    Log.Error(ex, "Failed sending batch after jobId {LastJobId}", lastJobId);
                    continue;
                }

                var jsonString = await response.Content.ReadAsStringAsync();

                var aiResponse = JsonSerializer.Deserialize<PrepareRecommendedResponse>(jsonString);


                if (aiResponse == null || !aiResponse.Success || aiResponse.results == null)
                {
                    Log.Error("From response "+aiResponse.results);
                    continue;
                }

                foreach (var res in aiResponse.results)
                {
                    if (res.match_score < MatchThreshold)
                        continue;

                    if (!int.TryParse(res.job_index, out int jobId))
                        continue;

                    recommendedJobs.Add(new RecommendedJobs
                    {
                        IsSent = true,
                        jobId = jobId,
                        MatchScore = res.match_score,
                        userId = userId
                    });
                }
            }

            if (recommendedJobs.Count > 0)
            {
                await unitOfWork.RecommendedJobs.AddRangeAsync(recommendedJobs);
                await unitOfWork.SaveChangesAsync();
            }

            if (recommendedJobs.Count < 10)
            {
                var searchKey = await unitOfWork.SearchKeyWords
                    .Where(x => x.cvId == cvId)
                    .Select(x => x.Name)
                    .ToListAsync();

                var jobsFromSearch = await GetNewJobsFromSerpApi(searchKey);
                await ProcessAiMatching(userId, skill, jobsFromSearch);
            }
        }
        //public async Task PrepareWithoutAi(int user, int cvId)
        //{
        //    var userCvData = await context.userCvDatas.FirstOrDefaultAsync(x => x.Id == user && x.cvId == cvId);
        //    var jobs = await context.Jobs.ToListAsync();
        //    List<RecommendedJobs> recommendedJobs = new();
        //    foreach (var job in jobs)
        //    {
        //        recommendedJobs.Add(new RecommendedJobs()
        //        {
        //            jobId = job.Id,
        //            MatchScore = 50,
        //            userId = user
        //        });
        //    }
        //    await context.recommendedJobs.AddRangeAsync(recommendedJobs);
        //    await context.SaveChangesAsync();
        //    if (recommendedJobs.Count < 10)
        //    {
        //        var searchKey = await context.searchKeyWords.Where(x => x.cvId == cvId).Select(x => x.Name).ToListAsync();
        //        await GetNewJobsFromSerpApi(searchKey);
        //    }
        //}
        //public async Task PrepareRecommendedJobs2(string user, int cvId)
        //{
        //    var userCvData = await context.userCvDatas.FirstOrDefaultAsync(x => x.UserId == user);

        //    var jobType = await context.UserPreferenceـJobs.Where(x => x.userId == userCvData.Id)
        //        .Select(x => x.JobType)
        //        .ToListAsync();
        //    var userEx = await context.userPreferences.Where(x => x.userId == userCvData.Id)
        //        .Select(x => x.JobLevel).FirstOrDefaultAsync();

        //    var jobsQuery = await context.Jobs
        //        .Where(x => x.IsActive && (jobType.Contains(x.JobType) || x.JobLevel == userEx))
        //        .Select(x => x.Description)
        //        .ToListAsync();
        //    var userSkills = await context.userSkills
        //        .Where(x => x.UserId == userCvData.Id && x.cvId == cvId && !x.CV.IsDeleted)
        //        .Select(x => x.Skills.SkillName)
        //        .ToListAsync();



        //    if (jobsQuery.Count < 5)
        //    {
        //        var searchKeys = await context.searchKeyWords
        //            .Where(x => x.cvId == cvId).Select(x => x.Name).ToListAsync();


        //        await GetNewJobsFromSerpApi(searchKeys);


        //        jobsQuery = await context.Jobs
        //        .Where(x => x.IsActive && (jobType.Contains(x.JobType) || x.JobLevel == userEx))
        //        .Select(x => x.Description)
        //        .ToListAsync();
        //    }


        //  //  await ProcessAiMatching(userCvData.Id, userSkills, jobsQuery);
        //}


        private async Task ProcessAiMatching(int userId, List<string> skills, List<Job> jobs)
        {
            if (!jobs.Any()) return;

            var user = await unitOfWork.UserCvData .GetActiveUserWithCvById(userId);
            if (user == null)
                return;
            var cv_text = $"job_title: {user.JobTitle}, Skills: {string.Join(",", skills)}";

            var matchUrl = configuration["matchScoreUrl"];
            using var httpClient = httpClientFactory.CreateClient();
            var cvs = new AiUserRequest() { cv_name = user.CV.FileName, cv_text = cv_text, user_id = user.Id };

            var existingJobIds = await unitOfWork.RecommendedJobs
                   .Where(r => r.userId == userId)
                   .Select(r => r.jobId)
                   .ToListAsync();

            var jobIds = jobs.Select(x => x.Id).ToList();
            var jobSkills = await unitOfWork.Jobs .GetJobSkills(jobIds);

            var recommendedToAdd = new List<RecommendedJobs>();
            foreach (var job in jobs)
            {
                var currentJobSkills = string.Join(", ", jobSkills.Where(x => x.JobId == job.Id).Select(x => x.Skills));
                var jobData = $"Description: {job.Description}. Required Skills: {currentJobSkills}";
                var request = new AiMatcheRequest
                {
                    cvs = new List<AiUserRequest>() { cvs }
                ,
                    job_description = jobData
                };
                var response = await httpClient.PostAsJsonAsync(matchUrl, request);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<AiResponseDto>();
                    if (data?.results == null) return;

                    foreach (var i in data.results)
                    {
                        if (i.match_percentage >= 65)
                        {
                            recommendedToAdd.Add(new RecommendedJobs()
                            {
                                MatchScore=i.match_percentage,
                                jobId=job.Id,
                                userId=userId,
                                IsSent=false
                            });
                        }
                    }

                }
            }
            try
            {
                await unitOfWork.RecommendedJobs.AddRangeAsync(recommendedToAdd);
                await unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task CleanUpOldJob()
        {
            var jobs = await unitOfWork.Jobs.Where(x => x.Local == false && x.IsActive == true &&
            x.CreatedAt <= DateTime.UtcNow.AddMonths(-2))
                .ToListAsync();
            foreach (var i in jobs)
                i.IsActive = false;
            await unitOfWork.SaveChangesAsync();
        }
    }
}
