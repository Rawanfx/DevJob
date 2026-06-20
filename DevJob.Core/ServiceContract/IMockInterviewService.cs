using DevJob.Application.DTOs.MockInterview;

namespace DevJob.Application.ServiceContract
{
    public interface IMockInterviewService
    {
        Task<StartInterviewResult> StartInterview (string userId, StartInterviewDto startInterviewDto);
    }
}
