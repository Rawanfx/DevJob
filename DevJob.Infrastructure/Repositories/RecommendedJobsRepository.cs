using DevJob.Application.DTOs.Jobs;
using DevJob.Domain.Entities;
using DevJob.Application.Repository_Contract;
using DevJob.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevJob.Infrastructure.Repositories
{
    public class RecommendedJobsRepository : RepositoryGeneric<RecommendedJobs>, IRecommendedJobRepository
    {
        private readonly AppDbContext context;
        public RecommendedJobsRepository(AppDbContext context) : base(context) =>
            this.context = context;

        public async Task<List<RecommendedJobDto>> GetRecommendedJobForUser(List<int> userCvData)
        {
            var recommendedJobs = await context.RecommendedJobs
               .Include(x => x.Job1)
               .Where(x => userCvData.Contains(x.userId) && x.Job1.IsActive)
               .Select(x => new RecommendedJobDto
               {
                   job_id = x.jobId,
                   apply_Link = x.Job1.ApplyLink,
                   CompanyName = x.Job1.CompanyName,
                   DeadLine = x.Job1.DeadLine,
                   Desctiption = x.Job1.Description,
                   EmploymentType = x.Job1.EmploymentType,
                   JobLevel = x.Job1.JobLevel,
                   JobType = x.Job1.JobType,
                   Location = x.Job1.Location,
                   MatchScore = x.MatchScore,
                   PostedAt = x.Job1.PostedAt,
                   Title = x.Job1.Title
               }
               ).Distinct()
               .ToListAsync();
            return recommendedJobs;
        }

        public async Task<HashSet<string>> RecommendedJobs()
        {
            return await context.RecommendedJobs
         .Select(x => $"{x.jobId}_{x.userId}")
         .ToHashSetAsync();
        }
    }
}
