using DevJob.Application.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace DevJob.Infrastructure.Validation
{
    public class UploadLogoValidate:AbstractValidator<UploadPictureDTO>
    {
        public UploadLogoValidate()
        {
            RuleFor(x => x.logo)
                .Must(Extension)
                .WithMessage("Extension must be .png / jpeg")
                .Must(Length)
                .WithMessage("Logo size is too long");
        }
        private bool Extension(IFormFile logo)
        {
            var extension = Path.GetExtension(logo.FileName);
            if (extension != ".jpeg" && extension != ".png" && extension != ".jpg")
                return false;
            return true;
        }
        private bool Length(IFormFile file)
        {
            if (file.Length > 2 * 1024 * 1024)
                return false;
            return true;
        }
    }
}
