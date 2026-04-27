using DevJob.Application.DTOs.Company;
using DevJob.Domain.Entities;
using DevJob.Domain.Enums;
using FluentValidation;
using Google.Apis.Util;
namespace DevJob.Infrastructure.Validation
{
    public class JobValidation:AbstractValidator<PostJobDTO>
    {
        public JobValidation()
        {
            RuleFor(x => x.MinimumExperience)
              .GreaterThanOrEqualTo(x => x.MaximumExperience)
              .When(x => x.MaximumExperience.HasValue && x.MaximumExperience.HasValue);

            RuleFor(x => x.DeadLine)
                .GreaterThanOrEqualTo(x => x.CreatedAt);
               

            RuleFor(x => x.JobLevel)
                .IsInEnum();

            RuleFor(x => x.JobType)
                .IsInEnum();

            RuleFor(x => x.EmploymentType)
                .IsInEnum();
        }
        
    }
}
