
using System.Text.Json.Serialization;

namespace DevJob.Application.DTOs.MockInterview
{
    public class FollowUpResult
    {
        [JsonPropertyName("need_follow_up")]
        public bool NeedFollowUp { get; set; }
        [JsonPropertyName("follow_up_question")]
        public string FollowUpQuestion { get; set; }
        [JsonPropertyName("reason")]
        public string Reason { get; set; }
    }
}
