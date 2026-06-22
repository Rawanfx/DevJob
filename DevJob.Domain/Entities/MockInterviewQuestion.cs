using System.ComponentModel.DataAnnotations.Schema;

namespace DevJob.Domain.Entities
{
    public class MockInterviewQuestion
    {
        public int Id { get; set; }
        [ForeignKey(nameof (MockInterview))]
        public int MockInterviewId { get; set; }
        public string Question { get; set; }
        public string? Answer { get; set; }
        public string? AIFeedback { get; set; }
        public int OrderNumber { get; set; }
        public bool IsFollowUp { get; set; }
        public int? ParentQuestionId { get; set; }
        public int? AnsweredInSeconds { get; set; }
        public bool TimedOut { get; set; }
        public float? OverallConfidence { get; set; }
        
        //final Results
        public float? FinalScore { get; set; }
        public float? FinalAvgEyeContact { get; set; }
        public float? FinalSpeechConfidence { get; set; }
        public string? FinalFeedBack { get; set; }//from gemini
        public string? FinalDominantEmotion { get; set; }
        public string? CorrectPoints { get; set; }    
        public string? MissingPoints { get; set; }   
        public string? SuggestedAnswer { get; set; }  
        public MockInterview MockInterview { get; set; }
        public MockInterviewQuestion ParentQuestion { get; set; }
        public FaceAnalysisResult FaceAnalysisResult { get; set; }
        public SpeechAnalysisResult SpeechAnalysisResult { get; set; }
        public ToneAnalysisResult ToneAnalysisResult { get; set; }
    }
}
