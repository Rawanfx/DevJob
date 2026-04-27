using DevJob.Application.DTOs.Jobs;
using DevJob.Application.Repository_Contract;
using DevJob.Domain.Entities;
using DevJob.Infrastructure.Data;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace DevJob.Infrastructure.Repositories
{
    public class CompanyRepository : RepositoryGeneric<CompanyProfile>, ICompanyRepository
    {
        private readonly AppDbContext context;
        public CompanyRepository(AppDbContext context)
            : base(context)
        {
            this.context = context;
        }
        public async Task<List<GetApplicantDto>> ApplicantSearch(string item, int jobId) 
        { var searchTerm = item?.Trim().ToLower() ?? "";
            var query = context.UserJobs.Include(x => x.UserCvData)
                .ThenInclude(x=>x.UserSkills)
                .ThenInclude(x=>x.Skills)
                .Where(x => x.jobID == jobId);
            if (!string.IsNullOrEmpty(searchTerm)) 
            { 
                query = query.Where(x=> x.UserCvData.JobTitle.ToLower().Contains(item)||
                x.UserCvData.UserSkills.Any(y=>y.Skills.SkillName.ToLower().Contains(searchTerm))
                ); 
            }
            return await query.Select(x => new GetApplicantDto() { userId = x.userId, ApplyDate = x.AppliedAt, Name = x.UserCvData.Name, status = x.Status, }).Distinct().ToListAsync(); 
        }
    }
}
