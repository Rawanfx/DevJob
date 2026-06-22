using System.ComponentModel.DataAnnotations.Schema;

namespace DevJob.Domain.Entities
{
    public class SpeechAnalysisResult
    {
        public int Id { get; set; }

        [ForeignKey(nameof(MockInterviewQuestion))]
        public int MockInterviewQuestionId { get; set; }

        public string TranscribedText { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string SpeechPace { get; set; } = string.Empty;
        public float WordsPerMinute { get; set; }
        public int PauseCount { get; set; }
        public float ClarityScore { get; set; }

        public string? SuggestionsJson { get; set; }

        public MockInterviewQuestion MockInterviewQuestion { get; set; } = null!;
    }
}