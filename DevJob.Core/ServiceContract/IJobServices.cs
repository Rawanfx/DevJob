using DevJob.Application.DTOs;
using DevJob.Application.DTOs.Chat;
using DevJob.Application.DTOs.Cvs;
using DevJob.Application.DTOs.Company;
using DevJob.Application.DTOs.Jobs;

namespace DevJob.Infrastructure.Service
{
    public interface IJobServices
    {
        Task<PostJobResult> AddJob(PostJobDTO postJobDTO,string user);
        Task<ResultDto> GetJobsFromSerpApi(string userId);
        Task<ResultDto> Update(int jobId, string company, UpdateJobDto updateJobDto);
        Task<Application.DTOs.Cvs.ResponseDto> Delete(int jobId, string company);
        Task<List<AllJobsDto>> AllJobs(string company);
        Task<ResultDto> Apply(int jobId, string userId,int cvId);
        Task<DisplayRecommendedJobsDto> DisplayRecommendedjobs(string userId);
        Task<DisplayRecommendedJobsDto> DisplayRecommendedjobs2(string userId);
        Task<GetApplicantResultDto> GetApplicants(int jobId,string companyId);
        Task<UpdateStatusResultDto> UpdateStatus(UpdateStatusDto updateStatusDto,string company);
        Task<List<string>> GetAllSkills();
        Task<ApplicantsCountResult> ApplicantsCount(string company,int jobId);
        Task<UserPrefereResultDto> AddUserPrefare(UserPrefareDto userPrefareDto, string user);
        Task<ComapnyCountResult> CompanyCount(string user);
        Task<AddSavedJobsResult> AddSavedJobs(AddSavedJobsDto addSavedJobsDto,string appUser);
        Task<DisplaySavedJobDtoResult> DisplaySavedJob(string appUser);
        Task<List<RecommendedJobDto>> DisplayAllJobs();
        Task<List<RecommendedJobDto>> JobSearch(string item);
        Task<DisplaySavedJobDtoResult> SavedJobSearch(string item, string user);

    }
}