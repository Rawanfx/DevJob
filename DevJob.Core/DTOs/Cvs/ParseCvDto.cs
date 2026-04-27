using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevJob.Application.DTOs.Cvs
{
    public class ParseCvDto
    {
        public IFormFile file { get; set; }
        public string url { get; set; }
    }
}
