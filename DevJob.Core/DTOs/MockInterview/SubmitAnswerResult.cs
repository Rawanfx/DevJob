using DevJob.Application.DTOs.MockInterview;

public class SubmitAnswerResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public bool InterviewCompleted { get; set; }
    public string Upload { get; set; }
    public string VideoId { get; set; }
    public QuestionDto? NextQuestion { get; set; }
}