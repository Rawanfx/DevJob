using DevJob.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
namespace DevJob.Domain.Entities
{
    public class UserPreference
    {
        public int Id { get; set; }
        public decimal MinimumSalary { get; set; }
        [ForeignKey (nameof (UserCvData1))]
        public int userId { get; set; }
        public UserCvData UserCvData1 { get; set; }
        public JobLevel JobLevel { get; set; }
    }
}
