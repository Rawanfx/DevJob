using DevJob.Application.DTOs.Jobs;
using DevJob.Application.ServiceContract;
using DevJob.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevJob.Infrastructure.Service
{
    public class SkillsService
    {
        private readonly IUnitOfWork unitOfWork;
        public SkillsService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<List<string>> GetUserSkills (string user)
        {
            var userCvData = await unitOfWork.UserCvData.Where(x => x.UserId == user).Select(x=>x.Id).ToListAsync();

            if (userCvData == null)
            {
                throw new KeyNotFoundException("User Not found");
            }


            var skillIds =await unitOfWork.UserSkills
                                  .Where(x => userCvData.Contains(x.UserId))
                                  .Select(x => x.SkillId).ToListAsync();

            var skillsNames = await unitOfWork.Skills
                                           .Where(x => skillIds.Contains(x.Id))
                                           .Select(x => x.SkillName) 
                                           .ToListAsync();

            return skillsNames;

        }
     
    }
}
