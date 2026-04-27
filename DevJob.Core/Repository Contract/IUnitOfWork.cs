using DevJob.Domain.Entities;

namespace DevJob.Application.ServiceContract
{
    public interface IUnitOfWork
    {
        Task BeginTransaction();
        Task RollBackAsync();
        Task CommitAsync();
        Task<int> SaveChangesAsync();
        IRepository<chats> Chats { get; }
        IRepository<CompanyProfile> CompanyProfile { get; }
        IRepository<Conversation> Conversations { get; }
        IRepository<CV> Cvs { get; }
        IRepository<Job> Jobs { get; }
        IRepository<Notification> Notifications { get; }
        IRepository<RecommendedJobs> RecommendedJobs { get; }
        IRepository<RequiredSkills> RequiredSkills { get; }
        IRepository<SavedJobs> SavedJobs { get; }
        IRepository<SearchKeyWord> SearchKeyWords { get; }
        IRepository<Skills> Skills { get; }
        IRepository<UserCvData> UserCvData { get; }
        IRepository<UserPreference> UserPreference { get; }
        IRepository<UserPreferenceـJobs> UserPreferenceـJobs { get; }
        IRepository<UserPrefernce_Skills> UserPrefernce_Skills { get; }
        IRepository<UserProfile> UserProfile{ get; }
        IRepository<UserSkills> UserSkills { get; }
        IRepository<UserJob> UserJob { get; }
    }
}
