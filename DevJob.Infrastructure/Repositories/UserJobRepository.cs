using DevJob.Domain.Entities;
using DevJob.Application.Repository_Contract;
using DevJob.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DevJob.Infrastructure.Repositories
{
    public class UserJobRepository : RepositoryGeneric<UserJob>, IUserJobRepository
    {
        private readonly AppDbContext context;
        public UserJobRepository(AppDbContext context) : base(context) =>
          this.context = context;
        public async Task<List<UserJob>> GetApplicantsForJob(int jobId)
        {
            var applicantsList = await context.UserJobs
                .Where(x => x.jobID == jobId)
                .Include(x => x.UserCvData)
                .ToListAsync();
            return applicantsList;
        }
    }
}
