using DevJob.Application.DTOs.Chat;
using DevJob.Application.Repository_Contract;
using DevJob.Application.ServiceContract;
using DevJob.Domain.Entities;
using DevJob.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DevJob.Infrastructure.Repositories
{
    public class ChatRepository:RepositoryGeneric<chats>,IChatRepository
    {
        private readonly AppDbContext context;
        public ChatRepository(AppDbContext context):base(context) =>
            this.context = context;

        public async Task<List<chats>?> GetLastMessage(List<int> conversationsIds)
        {
          var lastMessage=  await context.Chats
                .Where(x => conversationsIds.Contains(x.ConversationId) && !x.IsDelete)
                .GroupBy(x => x.ConversationId)
                .Select(g => g.OrderByDescending(m => m.date).FirstOrDefault())
                .ToListAsync();
            return lastMessage;
        }

        public async Task<Dictionary<int, int>?> GetUnreadCount(List<int> conversationIds,string user)
        {
            var unreadMessage = await context.Chats.Where(x => conversationIds.Contains(x.ConversationId)
         && !x.IsDelete
         && x.SenderId != user)
             .GroupBy(x => x.ConversationId)
             .Select(x => new { convId = x.Key, unread = x.Count(x => x.Isread == false) })
             .ToDictionaryAsync(x => x.convId, x => x.unread);
            return unreadMessage;
        }

        public async Task<List<ConversationData>> SearchByCompanyNameAsync(string item)
        {
            var conversations = await context.convesations
                     .Where(x => x.CompanyProfile1.CompanyName.Contains(item))
                     .Include(x => x.CompanyProfile1)
                     .Select(x => new ConversationData { Id = x.Id, Name = x.CompanyProfile1.CompanyName })
                     .ToListAsync();
            return conversations;
        }

        public async Task<List<ConversationData>> SearchByDeveloperNameAsync(string item)
        {
            var conversations = await context.convesations
                     .Where(x => x.UserCvData1.Name.Contains(item))
                     .Include(x => x.CompanyProfile1)
                     .Select(x => new ConversationData { Id = x.Id, Name = x.CompanyProfile1.CompanyName })
                     .ToListAsync();
            return conversations;
        }
    }
}
