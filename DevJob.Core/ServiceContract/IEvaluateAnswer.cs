using DevJob.Application.DTOs.MockInterview;
namespace DevJob.Application.ServiceContract
{
    public interface IEvaluateAnswer
    {
        Task<AnswerEvaluation> EvaluateAnswer(string question, string answer, string jobTitle, List<string> skills);
    }
}
