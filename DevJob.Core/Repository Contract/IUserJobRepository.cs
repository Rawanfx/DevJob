using DevJob.Domain.Entities;
using DevJob.Application.ServiceContract;

namespace DevJob.Application.Repository_Contract
{
    public interface IUserJobRepository:IRepository<UserJob>
    {
        Task<List<UserJob>> GetApplicantsForJob(int jobId);
    }
}
