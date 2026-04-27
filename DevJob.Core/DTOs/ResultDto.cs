using DevJob.Application.DTOs.InputDataFromJson;
using DevJob.Application.DTOs.Jobs;

namespace DevJob.Application.DTOs
{
    public class ResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
      //  public AllDataInput? AllDataInput { get; set; }
        public List<JobsDto>? jobsDtos { get; set; }
        public int jobCount { get; set; }
        
    }
}
