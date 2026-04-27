using DevJob.Application.DTOs.Company;
using DevJob.Domain.Entities;
using DevJob.Infrastructure.Data;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;

namespace DevJob.Infrastructure.Validation
{
    public class UpdateCompanyDataValidation:AbstractValidator<UpdateCompanyProfileDTO>
    {
        private readonly UserManager<ApplicationUser> userManager;
        public UpdateCompanyDataValidation(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            RuleFor(x => x.Name)
                .MustAsync(UniqueName);
          
        }
        private bool CheckLength(IFormFile logo)
        {
            if (logo.Length > 2 * 1024 * 1024)
                return false;
            return true;
        }
        private async Task<bool> UniqueName(UpdateCompanyProfileDTO dto,string name, CancellationToken ct)
        {
            var res = await userManager.FindByNameAsync(name);
            if (res == null)
                return true;
            return dto.Id == res.Id;
        }
       

    }
}
