using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DevJob.Application.DTOs.Company
{
    public class UpdateCompanyProfileDTO
    {
        public string? Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        [Url]
        public string? Website { get; set; }
        public string? Location { get; set; }
       
    }
}
