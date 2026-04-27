using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevJob.Application.DTOs.Jobs
{
    public class GetApplicantResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<GetApplicantDto> getApplicantDtos { get; set; }
    }
}
