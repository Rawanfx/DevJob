

namespace DevJob.Application.DTOs.AiRequest
{
    public class UserDto
    {
        public int userId { get; set; }
        public List<string> SkillsName { get; set; } = new();
    }
}
