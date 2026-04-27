using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevJob.Application.DTOs.Jobs
{
    public class ComapnyCountResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public CompanyCount  CompanyCount { get; set; }
    }
    public class CompanyCount
    {
        public int Applicants { get; set; }
        public int Hires { get; set; }
        public int ActiveJob { get; set; }
    }
}
