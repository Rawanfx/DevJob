using DevJob.Application.Repository_Contract;
using DevJob.Domain.Entities;
using DevJob.Infrastructure.Data;
using System.Linq.Expressions;

namespace DevJob.Infrastructure.Repositories
{
    public class InterviewVideoRepository : RepositoryGeneric<InterviewVideo>,  IInterviewVideoRepository
    {
        private readonly AppDbContext context;
        public InterviewVideoRepository(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public Task<bool> AnyAsync(Expression<Func<MockInterviewReport, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task<List<InterviewVideo>> GetByStatusAsync(VideoStatus status, int maxResults = 10)
        {
            return  context.InterviewVideos
       .Where(v => v.Status == status)
       .OrderBy(v => v.CreatedAt)
       .Take(maxResults)
       .ToList();
        }
        public Task UpdateAsync(InterviewVideo entity)
        {
            context.InterviewVideos.Update(entity);
            return Task.CompletedTask;
        }
    }
}
