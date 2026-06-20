using DevJob.Domain.Entities;
using DevJob.Application.ServiceContract;
using DevJob.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using DevJob.Application.Repository_Contract;

namespace DevJob.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext context;
        private IDbContextTransaction transaction;
        public UnitOfWork(AppDbContext context)
        {
            this.context = context;
            Chats = new ChatRepository(context);
            CompanyProfile = new CompanyRepository(context);
            Conversations = new ConversationRepository(context);
            Cvs = new RepositoryGeneric<CV>(context);
            Jobs = new JobRepository(context);
            Notifications = new RepositoryGeneric<Notification>(context);
            RecommendedJobs = new RecommendedJobsRepository(context);
            RequiredSkills = new RepositoryGeneric<RequiredSkills>(context);
            SavedJobs = new RepositoryGeneric<SavedJobs>(context);
            SearchKeyWords = new RepositoryGeneric<SearchKeyWord>(context);
            Skills = new RepositoryGeneric<Skills>(context);
            UserCvData = new UserCvDataRepository(context);
            UserPreference = new RepositoryGeneric<UserPreference>(context);
            UserPreferenceـJobs = new RepositoryGeneric<UserPreferenceـJobs>(context);
            UserPrefernce_Skills = new RepositoryGeneric<UserPrefernce_Skills>(context);
            UserProfile = new RepositoryGeneric<UserProfile>(context);
            UserSkills = new UserSkillRepository(context);
            UserJob = new UserJobRepository(context);
            MockInterview = new RepositoryGeneric<MockInterview>(context);
            MockInterviewQuestion = new RepositoryGeneric<MockInterviewQuestion>(context);
            SpeechAnalysisResult = new RepositoryGeneric<SpeechAnalysisResult>(context);
            FaceAnalysisResult = new RepositoryGeneric<FaceAnalysisResult>(context);
        }
        public IChatRepository Chats { get; }
        public ICompanyRepository CompanyProfile { get; }
        public IConversationRepository Conversations { get; }
        public IRepository<CV> Cvs { get; }
        public IJobRepository Jobs { get; }
        public IRepository<Notification> Notifications { get; }
        public IRecommendedJobRepository RecommendedJobs { get; }
        public IRepository<RequiredSkills> RequiredSkills { get; }
        public IRepository<SavedJobs> SavedJobs { get; }
        public IRepository<SearchKeyWord> SearchKeyWords { get; }
        public IRepository<Skills> Skills { get; }
        public IUserCvDataRepository UserCvData { get; }
        public IRepository<UserPreference> UserPreference { get; }
        public IRepository<UserPreferenceـJobs> UserPreferenceـJobs { get; }
        public IRepository<UserPrefernce_Skills> UserPrefernce_Skills { get; }
        public IRepository<UserProfile> UserProfile { get; }
        public IUserSkillsRepository UserSkills { get; }
        public IUserJobRepository UserJob { get; }

        public IRepository<MockInterview> MockInterview { get; }

        public IRepository<SpeechAnalysisResult> SpeechAnalysisResult { get; }

        public IRepository<FaceAnalysisResult> FaceAnalysisResult { get; }

        public IRepository<MockInterviewQuestion> MockInterviewQuestion { get; }

        public async Task BeginTransaction()=>
           transaction= await context.Database.BeginTransactionAsync();
        public async Task CommitAsync() =>
            await transaction.CommitAsync();
        public async Task RollBackAsync() =>
            await transaction.RollbackAsync();
        public async Task<int> SaveChangesAsync() =>
            await context.SaveChangesAsync();
    }
}
