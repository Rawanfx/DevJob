using DevJob.Application.DTOs.MockInterview;
using DevJob.Application.ServiceContract;
using DevJob.Domain.Entities;
using DevJob.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Mscc.GenerativeAI.Types;

namespace DevJob.Infrastructure.Service
{
    public class MockInterviewService : IMockInterviewService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILLMService geminiService;
        private readonly IStorageService storageService;
        private readonly IQuickTranscriptionService quickTranscriptionService;
        public MockInterviewService (IUnitOfWork unitOfWork,ILLMService geminiService,
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

                await unitOfWork.SaveChangesAsync();
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
                await unitOfWork.CommitAsync();

                var firstQuestion = interviewQuestions[0];
                var questionsR = await unitOfWork.MockInterviewQuestion.Where(x => x.MockInterviewId == interview.Id)
                    .Select(x => new 
                    {
                        x.Id,
                        x.Question,
                    }).ToListAsync();
                List<QuestionDto> questionDtoList = new List<QuestionDto>();
                foreach (var i in questionsR)
                {
                    questionDtoList.Add(new QuestionDto()
                    {
                        QuestionId = i.Id,
                        IsFollowUp = false,
                        Question = i.Question,
                        QuestionNumber = i.Id - firstQuestion.Id + 1
                    });
                }
                return new StartInterviewResult
                {
                    Success = true,
                    InterviewId = interview.Id,
                    Upload = uploadUrl,
                    Questions = questionDtoList,
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
            var foundInB2 = await storageService.DoesFileExistsAsync(video.Id.ToString());
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

        public async Task<SubmitAnswerResult> SubmitAnswerAndGetNextQuestion(Guid videoId, string userid)
        {
            var video = await unitOfWork.InterviewVideo.FirstOrDefaultAsync(x => x.Id == videoId);
            if (video == null)
                throw new Exception("Video not found");

            var uploadExists = await storageService.DoesFileExistsAsync(video.Id.ToString());
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
                video.Status = VideoStatus.Processing;
                video.UpdatedAt = DateTimeOffset.UtcNow;

                var transcribedText = await quickTranscriptionService.TranscribeQuickAsync(video.Id.ToString());

                var userCv = await unitOfWork.UserCvData.FirstOrDefaultAsync(x => x.UserId == userid && x.cvId == interview.CvId);
                if (userCv == null)
                    return new SubmitAnswerResult { Success = false, Message = "User not found" };

                var jobTitle = interview.Track;
                var skills = await unitOfWork.UserSkills.GetUserSkills(interview.CvId, userCv.Id);

                var evaluation = await geminiService.EvaluateAnswer(question.Question, transcribedText, jobTitle, skills);

                question.Answer = transcribedText;
                question.FinalScore = evaluation.Score;
                question.FinalFeedBack = evaluation.Feedback;
                question.CorrectPoints = evaluation.Correct_Points;
                question.MissingPoints = evaluation.Missing_Points;
                question.SuggestedAnswer = evaluation.Suggested_Answer;

                var nextQuestion = await unitOfWork.MockInterviewQuestion
                    .FirstOrDefaultAsync(q => q.MockInterviewId == interview.Id
                                           && q.OrderNumber == question.OrderNumber + 1);

                string? nextUploadUrl = null;
                string? nextVideoId = null;

                if (nextQuestion is not null)
                {
                    var newObjectKey = Guid.NewGuid();
                    var newStorageKey = $"videos/{newObjectKey}.mp4";

                    var nextVideo = new InterviewVideo
                    {
                        Id = newObjectKey,
                        QuestionId = nextQuestion.Id,
                        StorageKey = newStorageKey,
                    };
                    await unitOfWork.InterviewVideo.AddAsync(nextVideo);

                    nextUploadUrl = storageService.GeneratePresignedUploadUrl(newObjectKey.ToString());
                    nextVideoId = nextVideo.Id.ToString();
                }
                else
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
                            IsFollowUp = false,
                        },
                    Upload = nextUploadUrl,
                    VideoId = nextVideoId,
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
