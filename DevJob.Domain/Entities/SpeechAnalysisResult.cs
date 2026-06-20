using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevJob.Domain.Entities
{
    public class SpeechAnalysisResult
    {
        public int Id { get; set; }
        // PK
        [ForeignKey(nameof(MockInterviewQuestion))]
        public int MockInterviewQuestionId { get; set; }      // FK

        public string TranscribedText { get; set; }
        public float SpeechConfidence { get; set; }
        public string SpeechPace { get; set; }
        public float WordsPerMinute { get; set; }
        public int PauseCount { get; set; }
        public float ClarityScore { get; set; }
        public string SuggestionsJson { get; set; }

        public MockInterviewQuestion MockInterviewQuestion { get; set; }
    }
}
