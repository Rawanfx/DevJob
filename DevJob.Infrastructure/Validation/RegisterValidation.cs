using DevJob.Application.DTOs.Auth;
using DevJob.Infrastructure.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Data;
namespace DevJob.Application.Validation
{
    public class RegisterValidation:AbstractValidator<CompanyRegisterDTO>
    {
        private  AppDbContext appDbContext;
       public RegisterValidation(AppDbContext appDbContext)
        { 
            this.appDbContext=appDbContext;
          
           

            RuleFor(x => x.SerailNumber)
           .NotEmpty()
           .Matches("^[A-Za-z0-9\\-]{8,20}$").WithMessage("Serial number must be 8-20 alphanumeric characters.")
           .MustAsync(UniqueSerail).WithMessage("Serail number must be unique");

            RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required.")
            .MaximumLength(100);

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .Equal(x => x.Password).WithMessage("password and confirm password do not match");
                
        }
      
        private async Task<bool> UniqueName(string name, CancellationToken ct)
        {
            var res = await appDbContext.Company.AnyAsync(x => x.applicationUser.Name == name, ct);
            return res == false;
        }
       
        private async Task<bool> UniqueSerail(string number, CancellationToken ct)
        {
            var res = await appDbContext.Company.AnyAsync(x => x.SerailNumber == number,ct);
            return res == false;
        }
    }
}
