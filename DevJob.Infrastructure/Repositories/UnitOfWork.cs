using DevJob.Domain.Entities;
using DevJob.Application.ServiceContract;
using DevJob.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace DevJob.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext context;
        private IDbContextTransaction transaction;
        public UnitOfWork(AppDbContext context)
        {
            this.context = context;
            Chats = new RepositoryGeneric<chats>(context);
            CompanyProfile = new RepositoryGeneric<CompanyProfile>(context);
            Conversations = new RepositoryGeneric<Conversation>(context);
            Cvs = new RepositoryGeneric<CV>(context);
            Jobs = new RepositoryGeneric<Job>(context);
            Notifications = new RepositoryGeneric<Notification>(context);
            RecommendedJobs = new RepositoryGeneric<RecommendedJobs>(context);
            RequiredSkills = new RepositoryGeneric<RequiredSkills>(context);
            SavedJobs = new RepositoryGeneric<SavedJobs>(context);
            SearchKeyWords = new RepositoryGeneric<SearchKeyWord>(context);
            Skills = new RepositoryGeneric<Skills>(context);
            UserCvData = new RepositoryGeneric<UserCvData>(context);
            UserPreference = new RepositoryGeneric<UserPreference>(context);
            UserPreferenceـJobs = new RepositoryGeneric<UserPreferenceـJobs>(context);
            UserPrefernce_Skills = new RepositoryGeneric<UserPrefernce_Skills>(context);
            UserProfile = new RepositoryGeneric<UserProfile>(context);
            UserSkills = new RepositoryGeneric<UserSkills>(context);
            UserJob = new RepositoryGeneric<UserJob>(context);
        }
        public IRepository<chats> Chats { get; }
        public IRepository<CompanyProfile> CompanyProfile { get; }
        public IRepository<Conversation> Conversations { get; }
        public IRepository<CV> Cvs { get; }
        public IRepository<Job> Jobs { get; }
        public IRepository<Notification> Notifications { get; }
        public IRepository<RecommendedJobs> RecommendedJobs { get; }
        public IRepository<RequiredSkills> RequiredSkills { get; }
        public IRepository<SavedJobs> SavedJobs { get; }
        public IRepository<SearchKeyWord> SearchKeyWords { get; }
        public IRepository<Skills> Skills { get; }
        public IRepository<UserCvData> UserCvData { get; }
        public IRepository<UserPreference> UserPreference { get; }
        public IRepository<UserPreferenceـJobs> UserPreferenceـJobs { get; }
        public IRepository<UserPrefernce_Skills> UserPrefernce_Skills { get; }
        public IRepository<UserProfile> UserProfile { get; }
        public IRepository<UserSkills> UserSkills { get; }
        public IRepository<UserJob> UserJob { get; }

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
