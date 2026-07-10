namespace DevJob.Application.DTOs
{
    public class PrepareRecommendedRequest
    {
        public string cv_title { get; set; }
        public string cv_skills { get; set; }
        public List<RequestJobs>jobs { get; set; }

    }
    public class RequestJobs
    {
       public int id { get; set; }
       public string description { get; set; }
    }
}
