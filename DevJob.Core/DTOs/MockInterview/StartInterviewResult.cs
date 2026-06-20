namespace DevJob.Application.DTOs.MockInterview
{
    public class StartInterviewResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int InterviewId { get; set; }
        public int TimerSeconds { get; set; }
        public string Upload { get; set; }
        public QuestionDto FirstQuestion { get; set; }
    }
}
