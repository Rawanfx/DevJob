using System.Text.Json;
using DevJob.Application.DTOs;
using DevJob.Application.DTOs.MockInterview;
using DevJob.Application.ServiceContract;
using DevJob.Domain.Entities;

namespace DevJob.Infrastructure.Service
{
    public class InterviewReportService : IInterviewReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGeminiService geminiService;

        public InterviewReportService(
            IUnitOfWork unitOfWork,
            IGeminiService geminiService)
        {
            _unitOfWork = unitOfWork;
            this.geminiService = geminiService;
        }

        public async Task<MockInterviewReportDto> GenerateReportAsync(
            int mockInterviewId,
            CancellationToken cancellationToken = default)
        {
            // ── 1. Load interview ─────────────────────────────────────────────
            var interview = await _unitOfWork.MockInterview
                .FirstOrDefaultAsync(i => i.Id == mockInterviewId)
                ?? throw new InvalidOperationException("Interview not found.");

            // ── 2. Load all answered questions (main + follow-ups) ────────────
            var questions = await _unitOfWork.MockInterviewQuestion
                .GetAllAnsweredAsync(mockInterviewId);

            if (!questions.Any())
                throw new InvalidOperationException(
                    "No answered questions found for this interview.");

            // ── 3. Build per-question snapshots from the 3 result entities ────
            var snapshots = new List<QuestionAnalysisSnapshot>();

            foreach (var q in questions)
            {
                var speech = await _unitOfWork.SpeechAnalysisResult
                    .FirstOrDefaultAsync(
                        s => s.MockInterviewQuestionId == q.Id);

                var tone = await _unitOfWork.ToneAnalysisResult
                    .FirstOrDefaultAsync(
                        t => t.MockInterviewQuestionId == q.Id);

                var face = await _unitOfWork.FaceAnalysisResult
                    .FirstOrDefaultAsync(
                        f => f.MockInterviewQuestionId == q.Id);

                snapshots.Add(new QuestionAnalysisSnapshot
                {
                    // Content
                    Question = q.Question,
                    Answer = q.Answer ?? string.Empty,
                    FinalScore = q.FinalScore,
                    FinalFeedBack = q.FinalFeedBack,
                    CorrectPoints = q.CorrectPoints,
                    MissingPoints = q.MissingPoints,

                    // Speech — from SpeechAnalysisResult
                    WordsPerMinute = speech?.WordsPerMinute,
                    PauseCount = speech?.PauseCount,
                    ClarityScore = speech?.ClarityScore,
                    SpeechPace = speech?.SpeechPace,

                    // Tone — from ToneAnalysisResult
                    DominantEmotion = tone?.DominantEmotion,
                    StrainScore = tone?.StrainScore,

                    // Body Language — from FaceAnalysisResult
                    AvgEyeContactPct = face?.AvgEyeContactPct,
                    PoorPostureWindowPct = face?.PoorPostureWindowPct,
                    BrowTensionScore = face?.AvgBrowTensionScore,
                    BlinkRatePerMinute = face?.BlinkRatePerMinute,
                    DominantHeadMovementType = face?.DominantHeadMovementType,
                });
            }

            // ── 4. Call Groq to generate report ───────────────────────────────
            var groqResult = await geminiService.GenerateInterviewReportAsync(
                snapshots,
                interview.Track,
                cancellationToken);

            // ── 5. Persist report ─────────────────────────────────────────────
            var report = new MockInterviewReport
            {
                MockInterviewId = mockInterviewId,
                OverallScore = groqResult.OverallScore,
                CommunicationScore = groqResult.CommunicationScore,
                ConfidenceScore = groqResult.ConfidenceScore,
                BodyLanguageScore = groqResult.BodyLanguageScore,
                EmotionalProfile = groqResult.EmotionalProfile,
                SpeechProfile = groqResult.SpeechProfile,
                BodyLanguageSummary = groqResult.BodyLanguageSummary,
                Strengths = JsonSerializer.Serialize(groqResult.Strengths),
                AreasForImprovement = JsonSerializer.Serialize(
                    groqResult.AreasForImprovement),
                Recommendations = JsonSerializer.Serialize(
                    groqResult.Recommendations),
            };

            await _unitOfWork.MockInterviewReport.AddAsync(report);
            await _unitOfWork.SaveChangesAsync();

            // ── 6. Return DTO ─────────────────────────────────────────────────
            return new MockInterviewReportDto
            {
                ReportId = report.Id,
                MockInterviewId = mockInterviewId,
                OverallScore = report.OverallScore,
                CommunicationScore = report.CommunicationScore,
                ConfidenceScore = report.ConfidenceScore,
                BodyLanguageScore = report.BodyLanguageScore,
                EmotionalProfile = report.EmotionalProfile,
                SpeechProfile = report.SpeechProfile,
                BodyLanguageSummary = report.BodyLanguageSummary,
                Strengths = JsonSerializer
                    .Deserialize<List<string>>(report.Strengths) ?? [],
                AreasForImprovement = JsonSerializer
                    .Deserialize<List<string>>(
                        report.AreasForImprovement) ?? [],
                Recommendations = JsonSerializer
                    .Deserialize<List<string>>(report.Recommendations) ?? [],
                GeneratedAt = report.GeneratedAt,
            };
        }
    }
}