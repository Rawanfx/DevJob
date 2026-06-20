using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DevJob.Application.DTOs.MockInterview
{
    public class AnswerEvaluation
    {
        [JsonPropertyName("score")]
        public float Score { get; set; }
        [JsonPropertyName("feedback")]
        public string Feedback { get; set; }
        [JsonPropertyName("correct_points")]
        public string Correct_Points { get; set; }
        [JsonPropertyName("missing_points")]
        public string Missing_Points { get; set; }
        [JsonPropertyName("suggested_answer")]
        public string Suggested_Answer { get; set; }
    }

}
