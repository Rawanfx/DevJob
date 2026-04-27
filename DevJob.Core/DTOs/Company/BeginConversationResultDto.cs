
using System.Globalization;

namespace DevJob.Application.DTOs.Company
{
    public class BeginConversationResultDto
    {
        public int ConversationId { get; set; }
        public bool Succes { get; set; }
        public string Message { get; set; }
    }
}
