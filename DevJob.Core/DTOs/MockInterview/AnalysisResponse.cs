// AnalyzeResponse.cs
public class AnalyzeResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public int QuestionId { get; set; }
    public BodyLanguageResult? BodyLanguage { get; set; }
    public SpeechAnalysisDto? Speech { get; set; }
    public ToneAnalysisDto? Tone { get; set; }
}

public class BodyLanguageResult
{
    public float AvgEyeContactPct { get; set; }
    public float PoorPostureWindowPct { get; set; }
    public float AvgHeadMovementScore { get; set; }
    public float AvgBrowTensionScore { get; set; }
    public int TotalFaceTouchEvents { get; set; }
    public float BlinkRatePerMinute { get; set; }
    public string? DominantHeadMovementType { get; set; }

    public float FramesWithFaceDetectedPct { get; set; }
    public float FramesWithPoseDetectedPct { get; set; }
    public float FramesWithHandDetectedPct { get; set; }
    public string PerformanceOverTimeJson { get; set; } = string.Empty;
}

public class SpeechAnalysisDto
{
    public bool Success { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string SpeechPace { get; set; } = string.Empty;
    public float WordsPerMinute { get; set; }
    public int PauseCount { get; set; }
    public float ClarityScore { get; set; }
}

public class ToneAnalysisDto
{
    public bool Success { get; set; }
    public string DominantEmotion { get; set; } = string.Empty;
    public Dictionary<string, float> EmotionScores { get; set; } = new();
    public float PitchMean { get; set; }
    public float PitchStd { get; set; }
    public float EnergyMean { get; set; }
    public float SpeakingRate { get; set; }
}