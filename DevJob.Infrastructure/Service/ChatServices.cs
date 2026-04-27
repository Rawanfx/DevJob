using DevJob.Application.DTOs.Chat;
using DevJob.Application.DTOs.Company;
using DevJob.Domain.Entities;
using DevJob.Application.Repository_Contract;
using DevJob.Application.ServiceContract;
using DevJob.Infrastructure.Data;
using DevJob.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Linq;
using DevJob.Domain.Enums;

namespace DevJob.Infrastructure.Service
{
    public class ChatServices : IChatServices
    {
        private readonly IHubContext<MessageHub> hubContext;
        private readonly INotificationService notificationService;
        private IUnitOfWork unitOfWork;
        private readonly IUserCvDataRepository userCvDataRepository;
        private readonly IConversationRepository conversationRepository;
        private readonly IChatRepository chatRepository;
        public ChatServices(IHubContext<MessageHub> hubContext
           ,INotificationService notificationService
            ,IUnitOfWork unitOfWork
            ,IUserCvDataRepository userCvDataRepository
            ,IConversationRepository conversationRepository
            ,IChatRepository chatRepository)
        {
            this.hubContext = hubContext;
            this.unitOfWork = unitOfWork;
            this.notificationService = notificationService;
            this.userCvDataRepository = userCvDataRepository;
            this.chatRepository = chatRepository;
            this.conversationRepository = conversationRepository;
        }
        public async Task<DisplayAllConversationResult> displayAllConversationResult(string user)
        {
            var users = await userCvDataRepository.GetUserIds(user);
            var company = await unitOfWork.CompanyProfile.FirstOrDefaultAsync(x => x.ApplicationUser == user);
            if (users == null && company==null)
                throw new KeyNotFoundException("User not found") ;

            List<Conversation> conversations= new List<Conversation>();
            //company view
            if (users!=null &&users.Count > 0)
                conversations = await conversationRepository.GetAllConvesationWithCompanyView(users);
            //developer view
            else
            {
                List<int> companyIds = new List<int>() { company.Id };
                conversations = await conversationRepository.GetAllConvesationWithDeveloperView(companyIds);
            }
            var conversationsId = conversations
                    .Select(x => x.Id)
                    .ToList();

            var LastMessage = await chatRepository.GetLastMessage(conversationsId);

            var unreadMessage = await chatRepository.GetUnreadCount(conversationsId, user);
            List<DisplayAllConversation> dispalyConversation = new List<DisplayAllConversation>();
              foreach (var con in conversations)
                {
                    var lMessage = LastMessage.FirstOrDefault(x => x.ConversationId == con.Id);

                    if (users.Count > 0)
                    {
                        
                        dispalyConversation.Add(new DisplayAllConversation()
                        {
                            ConversationId = con.Id,
                            userName = con.CompanyProfile1.CompanyName,
                            LastMessage = lMessage?.Message ?? "No messages yet",
                            DateTime = lMessage?.date,
                            UnReadMessage = unreadMessage.GetValueOrDefault(con.Id, 0),
                            userId=con.CompanyProfile1.Id
                        });
                    }
                    else
                    {
                        dispalyConversation.Add(new DisplayAllConversation()
                        {
                            userId=con.UserId,
                            userName=con.UserCvData1.Name,
                            ConversationId = con.Id,
                            LastMessage = lMessage?.Message ?? "No messages yet",
                            DateTime = lMessage?.date,
                            UnReadMessage = unreadMessage.GetValueOrDefault(con.Id, 0),
                        });
                    }
                }
                return new DisplayAllConversationResult() {
                    displayAllConversations = dispalyConversation,
                    Success=true
                };
           
        }
        public async Task<BeginConversationResultDto> BedinConversation(BeginConversationDto beginConversationDto, string company)
        {
            var companyProfile = await unitOfWork.CompanyProfile.FirstOrDefaultAsync(x => x.ApplicationUser == company);
            if (companyProfile == null)
                return new BeginConversationResultDto() { Succes = false, Message = "Company not found" };
            var user = await unitOfWork.UserCvData.FirstOrDefaultAsync(x => x.Id == beginConversationDto.userId);
            if (user == null)
                return new BeginConversationResultDto() {Succes=false,Message="User not found" };
            var job = await unitOfWork.Jobs.FirstOrDefaultAsync(x => x.CompanyId == companyProfile.Id
            && x.Id == beginConversationDto.jobId);
            if (job == null)
                return new BeginConversationResultDto() { Succes = false, Message = "job not found" };
            var apply = await unitOfWork.UserJob.FirstOrDefaultAsync(x => x.userId == beginConversationDto.userId
            && x.jobID == beginConversationDto.jobId && x.Status == Status.Interview);
            if (apply == null)
                return new BeginConversationResultDto() { Succes = false, Message = "user hasn't applied" };

            var beginConversatinBefore = await unitOfWork.Conversations.FirstOrDefaultAsync(x => x.jobId == beginConversationDto.jobId
            && x.UserId == beginConversationDto.userId && x.CompanyId == beginConversationDto.companyId);
            if (beginConversatinBefore!=null)
                return new BeginConversationResultDto() { Succes = true, Message = "conversation has started",ConversationId= beginConversatinBefore.Id };
            Conversation convesation = new Conversation()
            {
                CompanyId = beginConversationDto.companyId,
                jobId = beginConversationDto.jobId,
                UserId = beginConversationDto.userId
            };
           
            await unitOfWork.Conversations.AddAsync(convesation);
            await unitOfWork.SaveChangesAsync();
            //signalR
            var allUsersId = await unitOfWork.UserCvData
                .Where(x => x.UserId == user.UserId)
                .Select(x => x.Id)
                .ToListAsync();
            var messages = await unitOfWork.Conversations.Where(x => allUsersId.Contains(x.Id)).CountAsync();
            var data = new
            {
                user=user.UserId,
                newMessage = messages
            };
            await hubContext.Clients.User(user.UserId).SendAsync("updateMessageNumber",data);
            return new BeginConversationResultDto()
            {
                Succes = true,
                Message = "conversation started",
                ConversationId = convesation.Id
            };
        }

        public async Task<DisplayChatContent> LoadChat(string userId, int conversationId)
        {

            var conversation = await conversationRepository.GetConversationWithCompanyProfile(conversationId);

                var developers = await unitOfWork.UserCvData.Where(x => x.UserId == userId).Select(x => x.Id).ToListAsync();

                if (conversation == null)
                {
                    return new DisplayChatContent { Success = false, Message = "Chat not found" };
                }
                bool isParticipant =developers.Contains(conversation.UserId) ||
                                     conversation.CompanyProfile1.ApplicationUser == userId;

                if (!isParticipant)
                {
                    return new DisplayChatContent { Success = false, Message = "You are not authorized to view this chat" };
                }
                var unreadMessages = await unitOfWork.Chats
                    .Where(x => x.ConversationId == conversationId && x.SenderId != userId && !x.Isread && !x.IsDelete)
                    .ToListAsync();

                if (unreadMessages.Any())
                {
                    foreach (var msg in unreadMessages)
                    {
                        msg.Isread = true;
                    }
                    await unitOfWork.SaveChangesAsync();
                }

                var messages = await unitOfWork.Chats
                    .Where(x => x.ConversationId == conversationId && !x.IsDelete)
                    .OrderBy(x => x.date)
                    .Select(x => new ChatContent
                    {
                        Message = x.Message,
                        DateTime = x.date,
                        SenderId = x.SenderId,
                        MessageId=x.Id
                    })
                    .ToListAsync();

                return new DisplayChatContent
                {
                    Success = true,
                    chatContents = messages
                };
        }
        public async Task<ResponseDto> DeleteMessage (string user, DeleteMessageDto deleteMessageDto)
        {
            var message = await unitOfWork.Chats.FirstOrDefaultAsync(x => x.Id == deleteMessageDto.messageId
            && x.ConversationId == deleteMessageDto.conversationId
            && user == x.SenderId);

            if (message == null || message.IsDelete)
                return new ResponseDto() { Success = false, Message = "Message Not Found" };

            var conversation = await conversationRepository
                .GetConversationWithCompanyProfileAndDeveloper(deleteMessageDto.conversationId);
            if (conversation==null)
                return new ResponseDto() { Success=false,Message="Conversation Not Found"};

            message.IsDelete = true;
            await unitOfWork.SaveChangesAsync();
            string x = conversation.UserCvData1.UserId;
            string y = conversation.CompanyProfile1.ApplicationUser;

            //signalR
            await hubContext.Clients.User(x).SendAsync("DeleteMessage", new
            {
                conversationId = conversation.Id,
                messageId = message.Id
            });
            await hubContext.Clients.User(y).SendAsync("DeleteMessage", new
            {
                conversationId = conversation.Id,
                messageId = message.Id
            });

            return new ResponseDto() { Success = true, Message = "Deleted" };
        }
        public async Task<ResponseDto> SendMessage (SendMessageDto sendMessageDto,string user)
        {
            Log.Information($"sending message parametars => userid : " +
                $"{user} message :{sendMessageDto.Message} conversationId :{sendMessageDto.conversationId}");
            var conversation = await conversationRepository
                .GetConversationWithCompanyProfileAndDeveloper(sendMessageDto.conversationId);
            if (conversation == null)
                return new ResponseDto() { Success = false, Message = "Conversation not found" };
            if (conversation.UserCvData1.UserId != user && conversation.CompanyProfile1.ApplicationUser != user)
                return new ResponseDto() { Success = false, Message = "You are not authorized to view this chat" };
           await unitOfWork.BeginTransaction();
            chats chats = new chats()
            {
                Message=sendMessageDto.Message,
                ConversationId=conversation.Id,
                Isread=false,
                SenderId=user,
                date=DateTime.UtcNow,
            };
            string resciever = "";
            string title = "";
            int recieverId;
            if (conversation.CompanyProfile1.ApplicationUser == user) {
                resciever = conversation.UserCvData1.UserId;
                recieverId = conversation.UserId;
                title =$"New Message From {conversation.CompanyProfile1.CompanyName}";
            }
            else {
                resciever = conversation.CompanyProfile1.ApplicationUser;
                recieverId = conversation.CompanyId;
                title = $"New Message From {conversation.UserCvData1.Name}";
            }
                try
                {
                await unitOfWork.Chats.AddAsync(chats);
                await unitOfWork.SaveChangesAsync();

                //send a message using signalR
                await hubContext.Clients.User(resciever).SendAsync("ReceiveMessage", new
                {
                    ConversationId = conversation.Id,
                    Message = sendMessageDto.Message,
                    SendAt=chats.date,
                    Reciever = resciever,
                    Sender=user
                });
                //send notification using one signal
                //save notification in database
                var res = await notificationService.SaveNotification(recieverId, sendMessageDto.Message,title);

               
               var sendRes= await notificationService.SendNotification(resciever, title, sendMessageDto.Message);
                if (!sendRes.Success)
                    throw new Exception(sendRes.Message);
                await unitOfWork.CommitAsync();
                return new ResponseDto() { Success = true,MessageId=chats.Id };
                }
                catch (Exception ex)
                {
                await unitOfWork.RollBackAsync();
                throw;
                }
        }
        public async Task<ResponseDto> UpdateMessage(UpdateMessageDto updateMessageDto, string user)
        {
            var conversation = await conversationRepository
                .GetConversationWithCompanyProfileAndDeveloper(updateMessageDto.conversationId);
            if (conversation == null)
                return new ResponseDto() { Success = false, Message = "Conversation not found" };
            var message = await unitOfWork.Chats.FirstOrDefaultAsync(x => x.Id == updateMessageDto.messageId
            && x.ConversationId == updateMessageDto.conversationId && x.SenderId == user && !x.IsDelete);
            if (message == null)
                return new ResponseDto() { Success = false, Message = "Message not found" };
            if (DateTime.UtcNow.Subtract(message.date).TotalMinutes > 15)
                return new ResponseDto() { Success = false, Message = "Time limit exceeded (15 mins)" };
            message.Message = updateMessageDto.newMessage;
            await unitOfWork.SaveChangesAsync();
            
            string x = conversation.UserCvData1.UserId;
            string y = conversation.CompanyProfile1.ApplicationUser;

            //signalR
            await hubContext.Clients.Users(x,y).SendAsync("UpdateMessage", new
            {
                conversationId = conversation.Id,
                messageId = message.Id,
                newMessage = updateMessageDto.newMessage
            });
            
            return new ResponseDto() { Success = true, Message = "Updated" };
        }
        private async Task<List<ChatSummaryDto>>GetLastMessages(List<ConversationData> conversations)
        {
            var conversationIds = conversations
                 .Select(x => x.Id)
                 .ToList();

            var LastMessages = await unitOfWork.Chats
                .Where(x => conversationIds.Contains(x.ConversationId) && !x.IsDelete)
                .OrderByDescending(x => x.date)
                .Select(x => new { x.ConversationId, x.Message, x.date })
                .ToListAsync();
            List<ChatSummaryDto> chatSummary = new();
            foreach (var i in conversations)
            {
                var lastMessage = LastMessages.FirstOrDefault(x => x.ConversationId == i.Id);
                chatSummary.Add(new ChatSummaryDto()
                {
                    ConversationId = i.Id,

                    date = lastMessage?.date,
                    Message = lastMessage?.Message,
                    UserName = i.Name
                });
            }
            return chatSummary;
        }
        public async Task<ChatSummaryResult> ConversationSearch(string item, string user)
        {
            var developer = await unitOfWork.UserCvData.FirstOrDefaultAsync(x => x.UserId == user);
            var company = await unitOfWork.CompanyProfile.FirstOrDefaultAsync(x => x.ApplicationUser == user);
            if (developer == null && company == null)
                return new ChatSummaryResult() { Success = false, Message = "Not Found Conversation" };
            //developer search for company name
            ChatSummaryResult result = new ChatSummaryResult();
            result.Success = true;
            
            if (developer != null)
            {
                var conversations = await chatRepository.SearchByCompanyNameAsync(item);

                var chatSummary =await GetLastMessages(conversations);
                result.chatSummaryDto = chatSummary;
            }
            else
            {
                var conversations = await chatRepository.SearchByDeveloperNameAsync(item);

                var chatSummary = await GetLastMessages(conversations);
                result.chatSummaryDto = chatSummary;
            }
            return result;
        }
    }
}
