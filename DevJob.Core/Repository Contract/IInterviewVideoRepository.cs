using DevJob.Application.ServiceContract;
using DevJob.Domain.Entities;
using System.Linq.Expressions;
namespace DevJob.Application.Repository_Contract
{
    public interface IInterviewVideoRepository:IRepository<InterviewVideo>
    {
        Task<List<InterviewVideo>> GetByStatusAsync(VideoStatus status, int maxResults = 10);
        Task UpdateAsync(InterviewVideo entity);
        // في IInterviewVideoRepository
        Task<bool> AnyAsync(Expression<Func<InterviewVideo, bool>> predicate);

        // في IMockInterviewReportRepository
        Task<bool> AnyAsync(Expression<Func<MockInterviewReport, bool>> predicate);
    }
}
