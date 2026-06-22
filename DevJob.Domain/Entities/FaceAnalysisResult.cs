using System.ComponentModel.DataAnnotations.Schema;

namespace DevJob.Domain.Entities
{
    public class FaceAnalysisResult
    {
        public int Id { get; set; }

        [ForeignKey(nameof(MockInterviewQuestion))]
        public int MockInterviewQuestionId { get; set; }

        public float AvgEyeContactPct { get; set; }
        public float PoorPostureWindowPct { get; set; }
        public float AvgHeadMovementScore { get; set; }
        public float AvgBrowTensionScore { get; set; }
        public int TotalFaceTouchEvents { get; set; }
        public float BlinkRatePerMinute { get; set; }
        public string? DominantHeadMovementType { get; set; }

        // مؤشرات جودة الكشف، مفيدة لو الوجه ما كانش ظاهر كويس في الفيديو
        public float FramesWithFaceDetectedPct { get; set; }
        public float FramesWithPoseDetectedPct { get; set; }
        public float FramesWithHandDetectedPct { get; set; }

        // الـ time_series الكامل، بيتخزن زي ما هو لأنه array مش columns
        public string PerformanceOverTimeJson { get; set; } = string.Empty;

        // ده هيتملي بعدين من خطوة توليد التوصيات بالـ LLM، مش من السكريبت نفسه
        public string? SuggestionsJson { get; set; }

        public MockInterviewQuestion MockInterviewQuestion { get; set; } = null!;
    }
}