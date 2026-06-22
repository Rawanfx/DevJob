public class GroqReportResult
{
    public float OverallScore { get; init; }
    public float CommunicationScore { get; init; }
    public float ConfidenceScore { get; init; }
    public float BodyLanguageScore { get; init; }
    public string EmotionalProfile { get; init; } = string.Empty;
    public string SpeechProfile { get; init; } = string.Empty;
    public string BodyLanguageSummary { get; init; } = string.Empty;
    public List<string> Strengths { get; init; } = [];
    public List<string> AreasForImprovement { get; init; } = [];
    public List<string> Recommendations { get; init; } = [];
}