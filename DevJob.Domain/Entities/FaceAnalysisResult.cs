using System.ComponentModel.DataAnnotations.Schema;

namespace DevJob.Domain.Entities
{
    public class FaceAnalysisResult
    {
        public int Id { get; set; }
        [ForeignKey (nameof (mockInterviewQuestion))]
        public int MockInterviewQuestionId { get; set; }
        public float AvgEyeContact { get; set; }
        public float AvgSmile { get; set; }
        public string DominantEmotion { get; set; }
        public string EmotionBreakdownJson { get; set; }
        public string PerformanceOverTimeJson { get; set; }
        public string SuggestionsJson { get; set; }
        public MockInterviewQuestion mockInterviewQuestion { get; set; }
    }
}
