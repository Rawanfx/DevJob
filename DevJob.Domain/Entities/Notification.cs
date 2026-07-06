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
        [ForeignKey(nameof (user))]
        public string UserId { get; set; }
        public ApplicationUser user { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
        public DateTime date { get; set; }

        public bool IsRead { get; set; } = false;
    }
}
