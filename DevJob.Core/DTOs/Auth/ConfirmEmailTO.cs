using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevJob.Application.DTOs.Auth
{
    public class ConfirmEmailTO
    {
        public string Email { get; set;}
        public string ConfirmationToken { get; set; }
    }

}
