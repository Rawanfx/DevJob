using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevJob.Application.DTOs.Jobs
{
    public class DisplaySavedJobDtoResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<DisplaySavedJobDto> displaySavedJobDtos { get; set; }
    }
    public class DisplaySavedJobDto
    {
        public int jobId { get; set; }
        public string jobName { get; set; }
        public List<string>Skills { get; set; }
        public string Location { get; set; }
        public bool IsExternal { get; set; }
        public string? ApplyLink { get; set; }
        public DateOnly SavedDate { get; set; }
    }
}
