using DevJob.Application.DTOs.Jobs;
using DevJob.Domain.Entities;
using DevJob.Application.ServiceContract;
using DevJob.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace DevJob.Infrastructure.Repositories
{
    public class JobRepository: RepositoryGeneric<Job>, IJobRepository
    {
        private readonly AppDbContext context;
        public JobRepository(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<Job?> GetActiveJob(int jobId)
        {
            var job = await context.Jobs
                .Include(x => x.companyProfile)
                .FirstOrDefaultAsync(x => x.Id == jobId && x.IsActive);
            return job;
        }

        public async Task<Dictionary<int, int>> GetCountOfApplicantForJob(int companyId)
        {
            var jobCount = await(from x in context.Jobs
                                 join
                                 y in context.UserJobs on x.Id equals y.jobID
                                 where x.CompanyId == companyId
                                 select x.Id)
                           .GroupBy(x => x)
                           .Select(x => new { id = x.Key, count = x.Count() })
                           .ToDictionaryAsync(x => x.id, x => x.count);
            return jobCount;
        }

        public async Task<List<JobWithSkillsDto>> GetJobSkills(List<int> jobIds)
        {
            var jobSkills = await context.RequiredSkills
               .Include(x => x.skills)
               .Where(x => jobIds.Contains(x.JobId))
               .Select(x => new JobWithSkillsDto {JobId= x.JobId,Skills= x.skills.SkillName })
               .ToListAsync();
            return jobSkills;
        }

        public async Task<Dictionary<int, List<string>>> GetRequiredSkills()
        {
            var requiredskills =await (from x in context.SavedJobs
                                  join y in context.RequiredSkills on x.jobId equals y.JobId
                                  join z in context.Skills on y.SkillId equals z.Id
                                  select new { x.jobId, z.SkillName }
                               ).
                               GroupBy(x => x.jobId)
                               .ToDictionaryAsync(x => x.Key, x => x.Select(y => y.SkillName).ToList());
            return requiredskills;
        }

        public async Task<Dictionary<int, double>> GetScoreForApplicant(int jobId)
        {
            var scores = await context.RecommendedJobs
               .Where(x => x.jobId == jobId)
               .GroupBy(x => x.userId)
               .Select(x => new {
                   userId = x.Key,
                   MatchScore = x.Max(y => y.MatchScore)
               })
               .ToDictionaryAsync(x => x.userId, x => x.MatchScore);
            return scores;
        }

        public async Task<List<RecommendedJobDto>> JobSearch(string item)
        {
            var result = await context.Jobs
                .Where(x => x.IsActive &&
                (x.Description.Contains(item)||
                (x.Title.Contains(item))||
                (x.companyProfile.CompanyName.Contains(item))||
                (x.RequiredSkills.Any(y=>y.skills.SkillName.Contains(item)))))
                .Select(x => new RecommendedJobDto() {
                    apply_Link = x.ApplyLink,
                    CompanyName=x.companyProfile.CompanyName,
                    Desctiption=x.Description,
                    EmploymentType=x.EmploymentType,
                    JobLevel=x.JobLevel,
                    JobType=x.JobType,
                   Title=x.Title,
                   job_id=x.Id,
                   Location=x.Location,
                   PostedAt=x.PostedAt,
                })
                .ToListAsync();
            return result;
        }
        public async Task<List<DisplaySavedJobDto>> SavedJobSearch(string item, List<int> userIds)
        {
            var jobSkills = await GetRequiredSkills();
            var result = await context.SavedJobs
                .Include(x=>x.Job1)
                .Where(x => x.Job1.IsActive &&
                userIds.Contains(x.userId)&&
                (x.Job1.Description.Contains(item) ||
                (x.Job1.Title.Contains(item)) ||
                (x.Job1.companyProfile.CompanyName.Contains(item)) ||
                (x.Job1.RequiredSkills.Any(y => y.skills.SkillName.Contains(item)))))
                .Select(x => new DisplaySavedJobDto()
                {
                jobId=x.jobId,
                jobName=x.Job1.Title,
                Location=x.Job1.Location,
                SavedDate=x.date,
                Skills = jobSkills.GetValueOrDefault(x.jobId,new List<string>())
                })
                .Distinct()
                .ToListAsync();
            return result;
        }
    }
}
