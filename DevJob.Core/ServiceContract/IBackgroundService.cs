using DevJob.Domain.Entities;

namespace DevJob.Application.ServiceContract
{
    public interface IBackgroundService
    {
        Task<List<Job>> GetNewJobsFromSerpApi(List<string>searchKey);
        Task<List<Job>> GetNewJobsFromSerpApi();
        Task CalculateMatchJobs();
        Task CalculateMatchingForNewJob(int jobId);
        Task PrepareRecommendedJobs(string user,int cvId);
      //  Task CalculateMatchJobsWithoutAi();
      //  Task PrepareWithoutAi(int user, int cvId);

    }
}
