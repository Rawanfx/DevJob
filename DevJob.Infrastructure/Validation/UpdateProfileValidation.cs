using DevJob.Application.DTOs.User;
using FluentValidation;

namespace DevJob.Infrastructure.Validation
{
    public class UpdateProfileValidation:AbstractValidator<UserProfileDTO>
    {
        public UpdateProfileValidation()
        {
            RuleFor(x => x.Github)
                .Must(x => IsValidGithub(x));
            RuleFor(x => x.LinkedIn)
                .Must(x => IsValidLinkedIn(x));
        }
        private bool IsValidGithub(string url)
        {
           return url.ToLower().Contains("github.com");
        }
        private bool IsValidLinkedIn(string url)
        {
            return url.ToLower().Contains("linkedin.com");
        }
    }
}
