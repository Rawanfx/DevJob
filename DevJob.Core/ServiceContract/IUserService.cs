using DevJob.Application.DTOs.Auth;
using DevJob.Application.DTOs.Jobs;
using DevJob.Application.DTOs.User;

namespace DevJob.Application.ServiceContract
{
    public interface IUserService
    {
        Task<UserDataDto> GetUserData(string Id);
        Task<UpdateUserDataDTO> UpdateUserData(UserProfileDTO dataDTO);
        Task<AuthResponse> ChangeEmail(string newEmail, string token,string email);
        //  Task<UploadLogoResponse> UploadLogo(UploadLogoDTO uploadLogoDTO);
        Task<ApplicantHistoryCountResult> ApplicantsCount(string user);
        Task<List<ApplicationHistoryData>> ApplicationHistoryData(string user);
        Task<UserCount> UserCount(string user);
    }
}
