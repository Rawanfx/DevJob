public class QuestionAnalysisSnapshot
{
    public string Question { get; init; } = string.Empty;
    public string Answer { get; init; } = string.Empty;
    public float? FinalScore { get; init; }
    public string? FinalFeedBack { get; init; }
    public string? CorrectPoints { get; init; }
    public string? MissingPoints { get; init; }

    // Speech
    public float? WordsPerMinute { get; init; }
    public int? PauseCount { get; init; }
    public float? ClarityScore { get; init; }
    public string? SpeechPace { get; init; }

    // Tone
    public string? DominantEmotion { get; init; }
    public float? StrainScore { get; init; }

    // Body Language
    public float? AvgEyeContactPct { get; init; }
    public float? PoorPostureWindowPct { get; init; }
    public float? BrowTensionScore { get; init; }
    public float? BlinkRatePerMinute { get; init; }
    public string? DominantHeadMovementType { get; init; }
}