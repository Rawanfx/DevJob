using DevJob.Domain.Entities;

namespace DevJob.Application.DTOs.Company
{
    public class UpdateCompanyProfileResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public CompanyProfile Data { get; set; }
    }
}
