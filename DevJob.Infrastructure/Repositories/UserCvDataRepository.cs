using DevJob.Application.DTOs.User;
using DevJob.Domain.Entities;
using DevJob.Application.Repository_Contract;
using DevJob.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DevJob.Infrastructure.Repositories
{
    public class UserCvDataRepository:RepositoryGeneric<UserCvData>,IUserCvDataRepository
    {
        private readonly AppDbContext context;
        public UserCvDataRepository(AppDbContext context) : base(context) =>
          this.context = context;

        public async Task<UserCvData?> GetActiveUserWithCvById(int id)
        {
            var user = await context.UserCvDatas
                .Include(x => x.CV)
                .FirstOrDefaultAsync(x => x.Id == id && !x.CV.IsDeleted);
            return user;
        }

        public async Task<List<GetUserDataDto>> GetUserData()
        {
            var rowData = await(from x in context.UserCvDatas
                                join y in context.CVs on x.cvId equals y.Id
                                join z in context.UserSkills on y.Id equals z.cvId
                                where y.IsDeleted == false
                                select new GetUserDataDto
                                {
                                    userId = x.Id,
                                    cvId = y.Id,
                                    cvName = y.FileName,
                                    jobTitle = x.JobTitle,
                                    skills = z.Skills.SkillName
                                }).ToListAsync();
            return rowData;
        }

        public async Task<UserCvData?> GetUserDataWithCv(int userId)
        {
            var userData = await context.UserCvDatas
                .Include(x => x.CV)
                .FirstOrDefaultAsync(x => x.Id == userId);
            return userData;
        }

        public async Task<List<int>> GetUserIds(string appUser)
        {
            var userIds = await context.UserCvDatas
                .Where(x => x.UserId == appUser)
                .Select(x => x.Id)
                .ToListAsync();
            return userIds;
        }
    }
}
