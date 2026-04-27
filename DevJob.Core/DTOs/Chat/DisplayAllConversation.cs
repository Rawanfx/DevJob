using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevJob.Application.DTOs.Chat
{
    public class DisplayAllConversation
    {
        public int ConversationId { get; set; }
        public string LastMessage { get; set; }
        public DateTime? DateTime { get; set; }
        public int UnReadMessage { get; set; }
        public int userId { get; set; }
        public string userName { get; set; }
    }
}
