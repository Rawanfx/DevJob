using DevJob.Application.ServiceContract;
using DevJob.Domain.Entities;
namespace DevJob.Application.Repository_Contract
{
    public interface IMockInterviewQuestionRepository:IRepository<MockInterviewQuestion>
    {
        Task<MockInterviewQuestion?> GetNextMainQuestionAsync(int interviewId, int afterOrderNumber);
        Task<List<MockInterviewQuestion>> GetAllAnsweredAsync(int mockInterviewId);
    }
}
