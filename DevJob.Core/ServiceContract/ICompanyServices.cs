using DevJob.Application.DTOs;
using DevJob.Application.DTOs.Company;
using DevJob.Application.DTOs.Jobs;
using DevJob.Application.DTOs.User;
using System.Globalization;
namespace DevJob.Application.ServiceContract
{
    public interface ICompanyServices
    {
       Task<UpdateCompanyProfileResult> Update(UpdateCompanyProfileDTO profileDTO,UploadPictureDTO pictureDTO);
        Task<UploadLogoResponse> UploadLogo(UploadLogoDTO uploadLogoDTO);
        Task<GetCompanyDTO> GetCompany(string Id);
       Task<int?> CompanyId(string userId);
        Task<GetApplicantResultDto> ApplicantSearch(string companyId, string item, int jobId);
  
    }
}
