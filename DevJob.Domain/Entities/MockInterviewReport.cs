using DevJob.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

public class MockInterviewReport
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [ForeignKey (nameof (MockInterview))]
    public int MockInterviewId { get; set; }

    // ── Scores ───────────────────────────────────────────────────────────
    public float OverallScore { get; set; }
    public float CommunicationScore { get; set; }
    public float ConfidenceScore { get; set; }
    public float BodyLanguageScore { get; set; }

    // ── LLM narrative fields ─────────────────────────────────────────────
    public string EmotionalProfile { get; set; } = string.Empty;
    public string SpeechProfile { get; set; } = string.Empty;
    public string BodyLanguageSummary { get; set; } = string.Empty;

    // ── JSON arrays (stored as string, deserialized on read) ─────────────
    public string Strengths { get; set; } = "[]";
    public string AreasForImprovement { get; set; } = "[]";
    public string Recommendations { get; set; } = "[]";

    public DateTimeOffset GeneratedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation
    public MockInterview MockInterview { get; set; } = null!;
}