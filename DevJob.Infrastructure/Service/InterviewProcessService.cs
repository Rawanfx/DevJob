using DevJob.Application.ServiceContract;
using DevJob.Domain.Entities;
using DevJob.Infrastructure.Repositories;
using DevJob.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Json;

namespace DevJob.Infrastructure.Service
{
    public class InterviewProcessService:IInterviewProcessService
    {
        private readonly IUnitOfWork unitOfWork;
        private IStorageService storageService;
        private readonly HttpClient httpClient;
        private string fastApiBaseUrl;
        public InterviewProcessService(IUnitOfWork unitOfWork
            ,IStorageService storageService
            ,HttpClient httpClient
            ,IOptions<FastApiSettings> options)
        {
            this.unitOfWork = unitOfWork;
            this.storageService = storageService;
            this.httpClient = httpClient;
            fastApiBaseUrl = options.Value.BaseUrl;
        }
        public async Task ProcessJobAsync(Guid videoId, CancellationToken cancellationToken)
        {
            var video = await unitOfWork.InterviewVideo.FirstOrDefaultAsync(v => v.Id == videoId);
            if (video is null) return;


            try
            {
            video.Status = VideoStatus.Processing;
            await unitOfWork.SaveChangesAsync();
                var question = await unitOfWork.MockInterviewQuestion
                    .FirstOrDefaultAsync(q => q.Id == video.QuestionId);
                if (question is null) throw new Exception($"Question {video.QuestionId} not found.");

                var cleanUrl = storageService.GetCleanVideoUrl(video.Id.ToString());

                var response = await httpClient.PostAsJsonAsync(
                    $"{fastApiBaseUrl}/analyze",
                    new { videoUrl = cleanUrl },
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                var result = await response.Content
                    .ReadFromJsonAsync<AnalyzeResponse>(cancellationToken: cancellationToken);

                if (result is null || !result.Success)
                    throw new Exception(result?.Message ?? "Analysis returned null");
                await unitOfWork.BeginTransaction();
                if (result.BodyLanguage is not null)
                    await unitOfWork.FaceAnalysisResult.AddAsync(new FaceAnalysisResult
                    {
                        MockInterviewQuestionId = question.Id,
                        AvgEyeContactPct = result.BodyLanguage.AvgEyeContactPct,
                        PoorPostureWindowPct = result.BodyLanguage.PoorPostureWindowPct,
                        AvgHeadMovementScore = result.BodyLanguage.AvgHeadMovementScore,
                        AvgBrowTensionScore = result.BodyLanguage.AvgBrowTensionScore,
                        TotalFaceTouchEvents = result.BodyLanguage.TotalFaceTouchEvents,
                        BlinkRatePerMinute = result.BodyLanguage.BlinkRatePerMinute,
                        DominantHeadMovementType = result.BodyLanguage.DominantHeadMovementType, 
                        FramesWithFaceDetectedPct = result.BodyLanguage.FramesWithFaceDetectedPct,
                        FramesWithPoseDetectedPct = result.BodyLanguage.FramesWithPoseDetectedPct,
                        FramesWithHandDetectedPct = result.BodyLanguage.FramesWithHandDetectedPct,
                        PerformanceOverTimeJson = result.BodyLanguage.PerformanceOverTimeJson,
                    });

                if (result.Speech is not null)
                    await unitOfWork.SpeechAnalysisResult.AddAsync(new SpeechAnalysisResult
                    {
                        MockInterviewQuestionId = question.Id,
                        TranscribedText = result.Speech.Text,
                        Language = result.Speech.Language,
                        SpeechPace = result.Speech.SpeechPace,
                        WordsPerMinute = result.Speech.WordsPerMinute,
                        PauseCount = result.Speech.PauseCount,
                        ClarityScore = result.Speech.ClarityScore,
                    });

                if (result.Tone is not null)
                    await unitOfWork.ToneAnalysisResult.AddAsync(new ToneAnalysisResult
                    {
                        MockInterviewQuestionId = question.Id,
                        DominantEmotion = result.Tone.DominantEmotion,
                        EmotionScoresJson = System.Text.Json.JsonSerializer.Serialize(result.Tone.EmotionScores),
                        PitchMean = result.Tone.PitchMean,
                        PitchStd = result.Tone.PitchStd,
                        EnergyMean = result.Tone.EnergyMean,
                        SpeakingRate = result.Tone.SpeakingRate,
                    });

                question.FinalAvgEyeContact = result.BodyLanguage?.AvgEyeContactPct;
                question.FinalDominantEmotion = result.Tone?.DominantEmotion;

                video.Status = VideoStatus.Completed;
                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                video.Status = VideoStatus.Failed;
                video.ErrorMessage = ex.Message;
                await unitOfWork.RollBackAsync();
                throw;
            }
        }
    }
}
