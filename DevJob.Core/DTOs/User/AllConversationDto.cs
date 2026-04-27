using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevJob.Application.DTOs.User
{
    public class AllConversationResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<AllConversationDto> allConversationDtos { get; set; }
    }

    public class AllConversationDto
    {
        public string CompanyName { get; set; }
        public string LastMassage { get; set; }
        public int date { get; set; }
        public int unReadMessages { get; set; }
    }
}
