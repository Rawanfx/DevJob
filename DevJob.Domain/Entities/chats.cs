
using System.ComponentModel.DataAnnotations.Schema;

namespace DevJob.Domain.Entities
{
    public class chats
    {
        public int Id { get; set; }
        [ForeignKey(nameof (convesation1))]
        public int ConversationId { get; set; }
        public Conversation convesation1 { get; set; }
        public string Message { get; set; }
        public DateTime date { get; set; }
        public bool Isread { get; set; }
        public string SenderId { get; set; }
        public bool IsDelete { get; set; } = false;
    }
}
