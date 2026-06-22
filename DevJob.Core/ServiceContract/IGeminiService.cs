
using DevJob.Application.DTOs.MockInterview;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DevJob.Application.ServiceContract
{
    public interface IGeminiService
    {
         Task<List<string>> GenerateGeneralInterviewQuestions(
      string jobTitle,
      List<string> skills,
      string level);
        Task<AnswerEvaluation> EvaluateAnswer(
           string question,
           string answer,
           string jobTitle,
           List<string> skills);

        Task<FollowUpResult> ShouldAskFollowUp(
            string question,
            string answer,
            float score,
            string jobTitle,
            List<string> skills);

        Task<InterviewReport> GenerateFinalReport(
            string jobTitle,
            List<QuestionAnswerPair> questionsAndAnswers,
            float overallScore,
            FaceSpeechSummary bodyLanguageSummary = null);
        Task<GroqReportResult> GenerateInterviewReportAsync(
    List<QuestionAnalysisSnapshot> snapshots,
    string jobTitle,
    CancellationToken cancellationToken = default);
    }
}
