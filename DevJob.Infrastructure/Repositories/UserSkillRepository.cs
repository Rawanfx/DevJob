using DevJob.Domain.Entities;
using DevJob.Application.Repository_Contract;
using DevJob.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DevJob.Infrastructure.Repositories
{
    public class UserSkillRepository:RepositoryGeneric<UserSkills>,IUserSkillsRepository
    {
        private readonly AppDbContext context;
        public UserSkillRepository(AppDbContext context) : base(context) =>
            this.context = context;

        public async Task<Dictionary<int, List<string>>> GetUserSkills(HashSet<int> userIds)
        {
            var userSkills = await(from x in context.Skills
                                   join y in context.UserSkills on x.Id equals y.SkillId
                                   where userIds.Contains(y.UserId)
                                   select new { y.UserId, x.SkillName }).GroupBy(x => x.UserId)
                                 .ToDictionaryAsync(x => x.Key, x => x.Select(k => k.SkillName).ToList());
            return userSkills;
        }

        public async Task<List<string>> GetUserSkills(int cv, int userId)
        {
            var skill = await context.UserSkills
               .Where(x => x.UserId == userId && x.cvId == cv && x.CV.IsDeleted == false)
              .Select(x => x.Skills.SkillName)
              .ToListAsync();
            return skill;
        }
    }
}
