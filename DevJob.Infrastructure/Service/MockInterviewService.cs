using DevJob.Application.DTOs.MockInterview;
using DevJob.Application.ServiceContract;
using DevJob.Domain.Entities;
using DevJob.Domain.Enums;

namespace DevJob.Infrastructure.Service
{
    public class MockInterviewService : IMockInterviewService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IGeminiService geminiService;
        private readonly IStorageService storageService;
        public MockInterviewService (IUnitOfWork unitOfWork,IGeminiService geminiService,
            IStorageService storageService)
        {
            this.unitOfWork = unitOfWork;
            this.geminiService = geminiService;
            this.storageService = storageService;
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
                await unitOfWork.CommitAsync();

                var firstQuestion = interviewQuestions[0];
                string objectKey = $"{interview.Id}.mp4";
                var uploadUrl = storageService.GeneratePresignedUploadUrl(objectKey);
                interview.VideoUrl = objectKey;
                await unitOfWork.SaveChangesAsync();
                return new StartInterviewResult
                {
                    Success = true,
                    InterviewId = interview.Id,
                    Upload=uploadUrl,
                    FirstQuestion = new QuestionDto
                    {
                        QuestionId = firstQuestion.Id,
                        Question = firstQuestion.Question,
                        QuestionNumber = 1,
                        TotalQuestions = interviewQuestions.Count,
                        IsFollowUp = false
                    }
                };
            }
            catch(Exception ex)
            {
                await unitOfWork.RollBackAsync();
                throw;
            }
           
        }

      


    }
}
