using DevJob.Domain.Enums;
namespace DevJob.Application.DTOs.Jobs
{
    public class GetApplicantDto
    {
        public int userId { get; set; }
        public string Name { get; set; }
        public decimal YearOfex { get; set; }
        public List<string> skillName { get; set; }
        public Status status { get; set; }
        public double MatchScore { get; set; }
        public DateOnly ApplyDate { get; set; }
    }
}
