namespace DevJob.Application.DTOs.Jobs
{
    public class JobsDto
    {
        public string title { get; set; }
        public string company_name { get; set; }
        public string location { get; set; }
        public detected_extensions detected_extensions { get;set;}
        public string description { get; set; }
        public List<jobHightlightsdata> ?job_highlights { get; set; }
        public List<ApplyOptionItem> apply_options { get; set; }
        public string job_id { get; set; }
    }
    public class detected_extensions
    {
        public string? posted_at { get; set; }
        public string? salary { get; set; }
        public bool work_from_home { get; set; }
        public string schedule_type { get; set; }
        public string qualifications { get; set; }
      
    }
    public class jobHightlightsdata
    {
        public string title { get; set; }
        public List<string> items { get; set; }
    }
    public class ApplyOptionItem
    {
        public string title { get; set; }
        public string link { get; set; }
    }
}
