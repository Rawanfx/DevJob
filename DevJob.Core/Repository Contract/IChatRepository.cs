using DevJob.Application.DTOs.Chat;
using DevJob.Application.ServiceContract;
using DevJob.Domain.Entities;

namespace DevJob.Application.Repository_Contract
{
    public interface IChatRepository:IRepository<chats>
    {
        Task<List<chats>?> GetLastMessage(List<int> conversationsIds);
        Task<Dictionary<int, int>?> GetUnreadCount(List<int> conversationIds,string user);
        Task<List<ConversationData>> SearchByCompanyNameAsync(string item);
        Task<List<ConversationData>> SearchByDeveloperNameAsync(string item);
    }
}
