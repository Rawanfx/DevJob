using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevJob.Application.DTOs.MockInterview
{
  
  public class QuestionDto
    {
        public int QuestionId { get; set; }
        public string Question { get; set; }
        public int QuestionNumber { get; set; }
        public int TotalQuestions { get; set; }
        public bool IsFollowUp { get; set; }
        public string FollowUpReason { get; set; }
    }
}
