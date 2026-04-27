namespace DevJob.Application.DTOs.Jobs
{
    public class DisplayRecommendedJobsDto
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; }
        public List<RecommendedJobDto> RecommendedJobs { get; set; }
    }
}
