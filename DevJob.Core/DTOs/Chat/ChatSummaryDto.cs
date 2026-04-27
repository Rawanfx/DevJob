namespace DevJob.Application.DTOs.Chat
{
    public class ChatSummaryDto
    {
        public string UserName { get; set; }
        public int ConversationId { get; set; }
        public string Message { get; set; }
        public DateTime? date { get; set; }
    }
    public class ChatSummaryResult
    {
        public List<ChatSummaryDto> chatSummaryDto { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
