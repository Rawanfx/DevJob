using DevJob.Application.DTOs.MockInterview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevJob.Application.ServiceContract
{
    public interface IInterviewReportService
    {
        Task<MockInterviewReportDto> GenerateReportAsync(
            int mockInterviewId, string userid,
            CancellationToken cancellationToken = default);
    }
}
