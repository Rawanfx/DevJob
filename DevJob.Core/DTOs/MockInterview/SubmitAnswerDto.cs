using Microsoft.AspNetCore.Http;

namespace DevJob.Application.DTOs.MockInterview
{
    public class SubmitAnswerDto
    {
        public int InterviewId { get; set; }
        public int QuestionId { get; set; }
        public IFormFile AudioFile { get; set; }
        public List<IFormFile>Images { get; set; }
    }
}
