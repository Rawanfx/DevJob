namespace DevJob.Application.DTOs.AiRequest
{
    public class AiMatcheRequest
    {
        public string job_description { get; set; }
        public List<AiUserRequest> cvs { get; set; }
    }
    public class AiUserRequest
    {
        public int user_id { get; set; }
      public string cv_name { get; set; }
      public string cv_text { get; set; }
    }
}
