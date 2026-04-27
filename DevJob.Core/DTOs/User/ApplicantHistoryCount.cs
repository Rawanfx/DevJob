namespace DevJob.Application.DTOs.User
{
    public class ApplicantHistoryCount
    {
        public int TotalApplied { get; set; }
        public int Interview { get; set; }
        public int Waiting { get; set; }
    }
    public class ApplicantHistoryCountResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ApplicantHistoryCount ApplicantHistoryCount { get; set; }
    }
}
