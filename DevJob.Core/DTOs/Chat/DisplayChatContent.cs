namespace DevJob.Application.DTOs.Chat
{
    public class DisplayChatContent
    {
        public bool Success { get; set; }
        public string Message { get;set;}
        public List<ChatContent> chatContents { get; set; }
    }
}
