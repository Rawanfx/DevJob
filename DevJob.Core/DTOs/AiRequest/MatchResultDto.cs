namespace DevJob.Application.DTOs.AiRequest
{
    public class AiResponseDto
    {
        public List<MatchResultDto> results { get; set; }
    }
    public class MatchResultDto
    {
        public int user_id { get; set; }
        public double match_percentage { get; set; }
        public string recommendation { get; set; }
    }
}
