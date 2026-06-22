using DevJob.Application.DTOs.MockInterview;
using DevJob.Application.ServiceContract;
using DevJob.Domain.Entities;
using DevJob.Domain.Enums;
using Mscc.GenerativeAI.Types;

namespace DevJob.Infrastructure.Service
{
    public class MockInterviewService : IMockInterviewService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IGeminiService geminiService;
        private readonly IStorageService storageService;
        private readonly IQuickTranscriptionService quickTranscriptionService;
        public MockInterviewService (IUnitOfWork unitOfWork,IGeminiService geminiService,
            IStorageService storageService,IQuickTranscriptionService quickTranscriptionService)
        {
            this.unitOfWork = unitOfWork;
            this.geminiService = geminiService;
            this.storageService = storageService;
            this.quickTranscriptionService = quickTranscriptionService;
        }
        public async Task<StartInterviewResult> StartInterview(
     string userId, StartInterviewDto startInterviewDto)
        {
            var cvData = await unitOfWork.UserCvData
                .FirstOrDefaultAsync(x => x.cvId == startInterviewDto.cvId && x.UserId == userId && x.CV.IsDeleted == false);

            if (cvData == null)
                return new StartInterviewResult
                {
                    Success = false,
                    Message = "CV not found"
                };

            var skills = await unitOfWork.UserSkills.GetUserSkills(startInterviewDto.cvId, cvData.Id);

            if (skills == null || skills.Count == 0)
                return new StartInterviewResult
                {
                    Success = false,
                    Message = "No skills found in your CV. Please upload a CV with skills first."
                };

            List<string> questions;

            questions = await geminiService.GenerateGeneralInterviewQuestions(startInterviewDto.Track, skills, startInterviewDto.Level);

            if (questions == null || questions.Count == 0)
                return new StartInterviewResult
                {
                    Success = false,
                    Message = "Failed to generate interview questions. Please try again."
                };
            await unitOfWork.BeginTransaction();
            try
            {
                var interview = new MockInterview
                {
                    UserId = userId,
                    CvId = startInterviewDto.cvId,
                    Track = startInterviewDto.Track,
                    Status = InterviewStatus.InProgress,
                    CreatedAt = DateTime.UtcNow,
                    Level = startInterviewDto.Level,
                   
                };
                await unitOfWork.MockInterview.AddAsync(interview);
                await unitOfWork.SaveChangesAsync();

                var interviewQuestions = questions.Select((q, index) =>
                    new MockInterviewQuestion
                    {
                        MockInterviewId = interview.Id,
                        Question = q,
                        OrderNumber = index + 1,
                        IsFollowUp = false
                    }).ToList();

                await unitOfWork.MockInterviewQuestion.AddRangeAsync(interviewQuestions);
                var objectKey = Guid.NewGuid();
                var storageKey = $"videos/{objectKey}.mp4";
                var interviewVideo = new InterviewVideo
                {
                    Id = objectKey,
                    QuestionId= interviewQuestions[0].Id,
                    StorageKey = storageKey,
                };
                await unitOfWork.InterviewVideo.AddAsync(interviewVideo);
                var uploadUrl = storageService.GeneratePresignedUploadUrl(objectKey.ToString());

                await unitOfWork.SaveChangesAsync();
                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();

                var firstQuestion = interviewQuestions[0];

                return new StartInterviewResult
                {
                    Success = true,
                    InterviewId = interview.Id,
                    Upload = uploadUrl,
                    FirstQuestion = new QuestionDto
                    {
                        QuestionId = firstQuestion.Id,
                        Question = firstQuestion.Question,
                        QuestionNumber = 1,
                        TotalQuestions = interviewQuestions.Count,
                        IsFollowUp = false
                    },
                    VideoId = interviewVideo.Id.ToString()
                };
            }
            catch(Exception ex)
            {
                await unitOfWork.RollBackAsync();
                throw;
            }
           
        }

      public async Task<ConfirmUploadVideoResult> ConfirmUpload (string videoId)
        {
            var video = await unitOfWork.InterviewVideo.FirstOrDefaultAsync(x => x.Id.ToString() == videoId);
            if (video == null)
                return new ConfirmUploadVideoResult() { Success=false,Message ="Vidoe not found"};
            if (video.Status != VideoStatus.PendingUpload)
                return new ConfirmUploadVideoResult() { Success = false };
            var foundInB2 = await storageService.DoesFileExistsAsync(video.StorageKey);
            if (foundInB2==false)
                return new ConfirmUploadVideoResult
                {
                    Success = false,
                    Message = "Security Alert: Video file was not found on the storage server. Please try uploading again."
                };
            video.Status = VideoStatus.Queued;
            await unitOfWork.SaveChangesAsync();
            return new ConfirmUploadVideoResult()
            {
                Success = true,
                Message = "Video verified successfully. Interview is now queued for AI processing."
            };
        }

        public async Task<SubmitAnswerResult> SubmitAnswerAndGetNextQuestion(Guid videoId,string userid)
        {
            var video = await unitOfWork.InterviewVideo.FirstOrDefaultAsync(x => x.Id == videoId);
            if (video == null)
                throw new Exception("Video not found");

            var uploadExists = await storageService.DoesFileExistsAsync(video.StorageKey);
            if (!uploadExists)
                throw new Exception("Video not found in storage");

            var question = await unitOfWork.MockInterviewQuestion.FirstOrDefaultAsync(q => q.Id == video.QuestionId);
            if (question is null)
                return new SubmitAnswerResult { Success = false, Message = "Associated question not found." };

            var interview = await unitOfWork.MockInterview.FirstOrDefaultAsync(i => i.Id == question.MockInterviewId);
            if (interview is null)
                return new SubmitAnswerResult { Success = false, Message = "Associated interview not found." };
            await unitOfWork.BeginTransaction();
            try
            {
                // ── 3) Mark queued, fast-transcribe this short clip ────
                video.Status = VideoStatus.Processing;
                video.UpdatedAt = DateTimeOffset.UtcNow;

                var transcribedText = await quickTranscriptionService.TranscribeQuickAsync(video.StorageKey);

                // ── 4) Context for scoring / follow-up ─────────────────
                var userId = await unitOfWork.UserCvData.FirstOrDefaultAsync(x => x.UserId == userid && x.cvId == interview.CvId);
                if (userId == null)
                    return new SubmitAnswerResult() { Success = false, Message = "User not found" };
                var jobTitle = interview.Track;
                var skills = await unitOfWork.UserSkills.GetUserSkills(interview.CvId, userId.Id);

                // ── 5) Score the answer, persist onto the question ─────
                var evaluation = await geminiService.EvaluateAnswer(question.Question, transcribedText, jobTitle, skills);

                question.Answer = transcribedText;
                question.FinalScore = evaluation.Score;
                question.FinalFeedBack = evaluation.Feedback;
                question.CorrectPoints = evaluation.Correct_Points;
                question.MissingPoints = evaluation.Missing_Points;
                question.SuggestedAnswer = evaluation.Suggested_Answer;
                // FinalAvgEyeContact / FinalSpeechConfidence / FinalDominantEmotion
                // stay null here — they're filled in later by the background
                // deep-analysis worker once body language + tone processing finish.

                // ── 6) Decide on a follow-up ────────────────────────────
                var followUp = await geminiService.ShouldAskFollowUp(
                    question.Question, transcribedText, evaluation.Score, jobTitle, skills);

                MockInterviewQuestion? nextQuestion;

                // Cap follow-up depth at one level: don't follow-up a follow-up.
                if (followUp.NeedFollowUp && !question.IsFollowUp)
                {
                    nextQuestion = new MockInterviewQuestion
                    {
                        MockInterviewId = question.MockInterviewId,
                        Question = followUp.FollowUpQuestion ?? "Can you elaborate on your previous answer?",
                        OrderNumber = question.OrderNumber,
                        IsFollowUp = true,
                        ParentQuestionId = question.Id,
                        AIFeedback = followUp.Reason,
                    };
                    await unitOfWork.MockInterviewQuestion.AddAsync(nextQuestion);
                }
                else
                {
                    // Resolve which "root" order number to resume from. If the
                    // question just answered was itself a follow-up, resume
                    // after its PARENT's position, not its own.
                    int rootOrderNumber = question.OrderNumber;
                    if (question.IsFollowUp && question.ParentQuestionId.HasValue)
                    {
                        var parent = await unitOfWork.MockInterviewQuestion
                            .FirstOrDefaultAsync(q => q.Id == question.ParentQuestionId.Value);
                        rootOrderNumber = parent?.OrderNumber ?? question.OrderNumber;
                    }

                    nextQuestion = await unitOfWork.MockInterviewQuestion
                        .GetNextMainQuestionAsync(question.MockInterviewId, rootOrderNumber);
                }

                if (nextQuestion is null)
                {
                    interview.Status = InterviewStatus.Completed;
                }

                await unitOfWork.SaveChangesAsync();


                await unitOfWork.CommitAsync();

                return new SubmitAnswerResult
                {
                    Success = true,
                    InterviewCompleted = nextQuestion is null,
                    NextQuestion = nextQuestion is null
                        ? null
                        : new QuestionDto
                        {
                            QuestionId = nextQuestion.Id,
                            Question = nextQuestion.Question,
                            IsFollowUp = nextQuestion.IsFollowUp,
                        },
                };
            }
            catch
            {
                await unitOfWork.RollBackAsync();
                throw;
            }
        }
    
    }
}
