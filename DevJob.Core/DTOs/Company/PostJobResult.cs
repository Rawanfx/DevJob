namespace DevJob.Application.DTOs.Company
{
    public class PostJobResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public PostJobDTO Job { get; set; }
    }
}
