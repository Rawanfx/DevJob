using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevJob.Application.DTOs.Chat
{
    public class UpdateMessageDto
    {
        public int messageId { get; set; }
        public int conversationId { get; set; }
        public string newMessage { get; set; }
    }
}
