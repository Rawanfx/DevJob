using DevJob.Application.DTOs.Jobs;
using DevJob.Application.ServiceContract;
using DevJob.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevJob.Application.Repository_Contract
{
    public interface ICompanyRepository:IRepository<CompanyProfile>
    {
        Task<List<GetApplicantDto>> ApplicantSearch(string item,int jobId);
    }
}
