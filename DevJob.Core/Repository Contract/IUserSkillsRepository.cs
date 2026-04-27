using DevJob.Domain.Entities;
using DevJob.Application.ServiceContract;
namespace DevJob.Application.Repository_Contract
{
    public interface IUserSkillsRepository:IRepository<UserSkills>
    {
        Task<List<string>> GetUserSkills(int cv, int userId);
        Task<Dictionary<int, List<string>>> GetUserSkills(HashSet<int> userIds);
    }
}
