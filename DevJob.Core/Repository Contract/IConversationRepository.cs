using DevJob.Domain.Entities;
using DevJob.Application.ServiceContract;

namespace DevJob.Application.Repository_Contract
{
    public interface IConversationRepository:IRepository<Conversation>
    {
        Task<List<Conversation>> GetAllConvesationWithCompanyView(List<int>userIds);
        Task<List<Conversation>> GetAllConvesationWithDeveloperView(List<int> userIds);
        Task<Conversation?> GetConversationWithCompanyProfile(int id);
        Task<Conversation?> GetConversationWithCompanyProfileAndDeveloper(int id);
    }
}
