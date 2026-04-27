using DevJob.Application.DTOs.Jobs;
using DevJob.Domain.Entities;
using DevJob.Application.ServiceContract;

namespace DevJob.Application.Repository_Contract
{
    public interface IRecommendedJobRepository:IRepository<RecommendedJobs>
    {
        Task<HashSet<string>> RecommendedJobs();
        Task<List<RecommendedJobDto>> GetRecommendedJobForUser(List<int> userIds);
    }
}
