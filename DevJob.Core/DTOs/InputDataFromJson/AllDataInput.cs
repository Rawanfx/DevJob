using DevJob.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Globalization;
using System.Text.Json.Serialization;

namespace DevJob.Application.DTOs.InputDataFromJson
{
    public class AllDataInput
    {
        public List<string> skills { get; set; }
        public string predicted_job_role { get; set; }
        public string job_title { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string linkedin { get; set; }
   
    }
}
