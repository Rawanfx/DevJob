using System.ComponentModel.DataAnnotations.Schema;

namespace DevJob.Domain.Entities
{
    public class ToneAnalysisResult
    {
        public int Id { get; set; }

        [ForeignKey(nameof(MockInterviewQuestion))]
        public int MockInterviewQuestionId { get; set; }

        public string DominantEmotion { get; set; } = string.Empty;
        public string EmotionScoresJson { get; set; } = string.Empty;
        public float PitchMean { get; set; }
        public float PitchStd { get; set; }
        public float EnergyMean { get; set; }
        public float SpeakingRate { get; set; }

    
        public MockInterviewQuestion MockInterviewQuestion { get; set; } = null!;
    }
}