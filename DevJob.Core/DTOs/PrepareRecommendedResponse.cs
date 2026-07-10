using System.Text.Json.Serialization;

namespace DevJob.Application.DTOs
{
    public class PrepareRecommendedResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        [JsonPropertyName("results")]
        public List<result> results { get; set; }
    }
    public class result
    {
        [JsonPropertyName("job_index")]
        public string job_index { get; set; }
        [JsonPropertyName("match_score")]
        public double match_score { get; set; }
    }
}
