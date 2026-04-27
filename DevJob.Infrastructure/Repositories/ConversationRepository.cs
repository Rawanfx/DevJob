using DevJob.Domain.Entities;
using DevJob.Application.Repository_Contract;
using DevJob.Application.ServiceContract;
using DevJob.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DevJob.Infrastructure.Repositories
{
    public class ConversationRepository:RepositoryGeneric<Conversation>,IConversationRepository
    {
        private readonly AppDbContext context;
        public ConversationRepository(AppDbContext context) : base(context) =>
            this.context = context;

        public async Task<List<Conversation>> GetAllConvesationWithCompanyView(List<int> userIds)
        {
           var conversations = await context.convesations
                   .Where(x => userIds.Contains(x.UserId))
                   .Include(x=>x.CompanyProfile1)
                   .ToListAsync();
            return conversations;
        }
        public async Task<List<Conversation>> GetAllConvesationWithDeveloperView(List<int> userIds)
        {
            var conversations = await context.convesations
                    .Where(x => userIds.Contains(x.UserId))
                    .Include(x => x.UserCvData1)
                    .ToListAsync();
            return conversations;
        }
        public async Task<Conversation?> GetConversationWithCompanyProfile(int id)
        {
            var conversation = await context.convesations
                 .Include(x => x.CompanyProfile1)
                 .FirstOrDefaultAsync(x => x.Id == id);
            return conversation;
        }
        public async Task<Conversation?> GetConversationWithCompanyProfileAndDeveloper(int id)
        {
            var conversation = await context.convesations
                 .Include(x => x.CompanyProfile1)
                 .Include(x=>x.UserCvData1)
                 .FirstOrDefaultAsync(x => x.Id == id);
            return conversation;
        }
    }
}
