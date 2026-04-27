using DevJob.Application.DTOs.Chat;
using DevJob.Application.DTOs.Company;
namespace DevJob.Application.ServiceContract
{
    public interface IChatServices
    {
        Task<DisplayAllConversationResult> displayAllConversationResult(string user);
        Task<BeginConversationResultDto> BedinConversation(BeginConversationDto beginConversationDto, string company);
        Task<DisplayChatContent> LoadChat(string user,int conversionId);
        Task<ResponseDto> SendMessage(SendMessageDto sendMessageDto, string user);
        Task<ResponseDto> DeleteMessage(string user, DeleteMessageDto deleteMessageDto);
        Task<ResponseDto> UpdateMessage(UpdateMessageDto updateMessageDto, string user);
        Task<ChatSummaryResult> ConversationSearch(string item, string user);
    }
}
