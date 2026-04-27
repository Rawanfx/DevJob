using DevJob.Application.DTOs.Jobs;
using DevJob.Domain.Entities;
using System.Runtime.CompilerServices;
namespace DevJob.Application.ServiceContract
{
    public interface IJobRepository:IRepository<Job>
    {
        Task<List<JobWithSkillsDto>> GetJobSkills(List<int> JobIds);
        Task<Dictionary<int, int>> GetCountOfApplicantForJob( int companyId);
        Task<Job?> GetActiveJob(int jobId);
        Task<Dictionary<int, double>> GetScoreForApplicant(int jobId);
        Task<Dictionary<int, List<string>>> GetRequiredSkills();
        Task<List<RecommendedJobDto>> JobSearch(string item);
        Task<List<DisplaySavedJobDto>> SavedJobSearch(string item, List<int> userIds);
    }
}
