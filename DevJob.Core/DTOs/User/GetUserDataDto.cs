namespace DevJob.Application.DTOs.User
{
    public class GetUserDataDto
    {
        public int userId { get; set; }
        public int cvId { get; set; }
        public string cvName { get; set; }
        public string jobTitle { get; set; }
        public string skills { get; set; }
    }
}
