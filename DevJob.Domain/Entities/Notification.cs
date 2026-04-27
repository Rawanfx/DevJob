using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevJob.Domain.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        [ForeignKey(nameof (UserCvData1))]
        public int? userId { get; set; }
        public UserCvData UserCvData1 { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
        public DateTime date { get; set; }
        [ForeignKey(nameof (CompanyProfile1))]
        public int? CompanyId { get; set; }
        public CompanyProfile CompanyProfile1 { get; set; }
        public bool IsRead { get; set; } = false;
    }
}
