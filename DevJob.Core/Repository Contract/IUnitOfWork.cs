using DevJob.Application.Repository_Contract;
using DevJob.Domain.Entities;

namespace DevJob.Application.ServiceContract
{
    public interface IUnitOfWork
    {
        Task BeginTransaction();
        Task RollBackAsync();
        Task CommitAsync();
        Task<int> SaveChangesAsync();
        IChatRepository Chats { get; }
        ICompanyRepository CompanyProfile { get; }
        IConversationRepository Conversations { get; }
        IRepository<CV> Cvs { get; }
        IJobRepository Jobs { get; }
        IRepository<Notification> Notifications { get; }
        IRecommendedJobRepository RecommendedJobs { get; }
        IRepository<RequiredSkills> RequiredSkills { get; }
        IRepository<SavedJobs> SavedJobs { get; }
        IRepository<SearchKeyWord> SearchKeyWords { get; }
        IRepository<Skills> Skills { get; }
        IUserCvDataRepository UserCvData { get; }
        IRepository<UserPreference> UserPreference { get; }
        IRepository<UserPreferenceـJobs> UserPreferenceـJobs { get; }
        IRepository<UserPrefernce_Skills> UserPrefernce_Skills { get; }
        IRepository<UserProfile> UserProfile{ get; }
        IUserSkillsRepository UserSkills { get; }
        IUserJobRepository UserJob { get; }
        IRepository<MockInterview> MockInterview { get; }
        IRepository<SpeechAnalysisResult> SpeechAnalysisResult { get; }
        IRepository<FaceAnalysisResult> FaceAnalysisResult { get; }
        IMockInterviewQuestionRepository MockInterviewQuestion  { get; }
        IInterviewVideoRepository InterviewVideo { get; }
        IRepository<ToneAnalysisResult> ToneAnalysisResult { get; }
        IRepository<MockInterviewReport> MockInterviewReport { get;  }
    }
}
