using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevJob.Application.DTOs.Jobs
{
    public class ApplicantsCountResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ApplicantsCount applicantsCount { get; set; }
    }
    public class ApplicantsCount
    {
        public int TotalApplicant { get; set; }
        public int TotalInterview { get; set; }
        public int TotalNew { get; set; }
        public int TotalReviewed { get; set; }
    }
}
