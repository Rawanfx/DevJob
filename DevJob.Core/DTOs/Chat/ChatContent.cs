namespace DevJob.Application.DTOs.Chat
{
    public class ChatContent
    {
        public DateTime DateTime { get; set; }
        public string SenderId { get; set; }
        public string Message { get; set; }
        public int MessageId { get; set; }
    }
}
