using DevJob.Application.Repository_Contract;
using DevJob.Domain.Entities;
using DevJob.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevJob.Infrastructure.Repositories
{
    public class MockInteviewQuestionRepository :RepositoryGeneric<MockInterviewQuestion>, IMockInterviewQuestionRepository
    {
        private readonly AppDbContext context;
        public MockInteviewQuestionRepository (AppDbContext context):base(context)
        {
            this.context = context;
        }
        public  Task<MockInterviewQuestion?> GetNextMainQuestionAsync(int interviewId, int afterOrderNumber)
        {
            var next = context.MockInterviewQuestions
        .Where(q => q.MockInterviewId == interviewId && !q.IsFollowUp && q.OrderNumber > afterOrderNumber)
        .OrderBy(q => q.OrderNumber)
        .FirstOrDefault();
            return Task.FromResult(next);
        }
        public async Task<List<MockInterviewQuestion>> GetAllAnsweredAsync(
    int mockInterviewId)
        {
            return  context.MockInterviewQuestions
                .Where(q =>
                    q.MockInterviewId == mockInterviewId &&
                    q.Answer != null)
                .OrderBy(q => q.OrderNumber)
                .ToList();
        }
    }
}
