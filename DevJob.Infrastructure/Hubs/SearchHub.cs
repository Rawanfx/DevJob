
using DevJob.Application.ServiceContract;
using DevJob.Infrastructure.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
namespace DevJob.Infrastructure.Hubs
{
  
    public class SearchHub:Hub
    {
        private readonly IJobServices jobServices;
        private readonly IChatServices chatServices;
        private readonly ICompanyServices companyServices;
        public SearchHub(IJobServices jobServices
            , IChatServices chatServices
            ,ICompanyServices companyServices)
        {
            this.jobServices = jobServices;
            this.chatServices = chatServices;
            this.companyServices = companyServices;
        }
        [Authorize]
        public async Task ConversationSearchForDeveloper(string item)
        {
            string user = Context.UserIdentifier;
            var result = await chatServices.ConversationSearch(item, user);
            await Clients.Caller.SendAsync("ConversationSearchResult", result);
        }
        public async Task JobSearch (string item)
        {
            var result = await jobServices.JobSearch(item);
            await Clients.Caller.SendAsync("JobSearchResult", result);
        }
        [Authorize(Roles ="Developer")]
        public async Task SavedJobSearch (string item)
        {
            string user = Context.UserIdentifier;
            var result = await jobServices.SavedJobSearch(item, user);
            await Clients.Caller.SendAsync("SavedJobSearchResult", result);
        }
        [Authorize(Roles ="Company")]
        public async Task ApplicantSearch (string item,int jobId)
        {
            string user = Context.UserIdentifier;
            var result = await companyServices.ApplicantSearch(user, item, jobId);
            await Clients.Caller.SendAsync("ApplicantSearch", result);
        }
    }
}
