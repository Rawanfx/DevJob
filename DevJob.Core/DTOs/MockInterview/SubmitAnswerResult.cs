using DevJob.Application.DTOs.MockInterview;

public class SubmitAnswerResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public bool InterviewCompleted { get; set; }
    public QuestionDto? NextQuestion { get; set; }
}