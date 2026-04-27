using DevJob.Application.DTOs.Cvs;
using Microsoft.AspNetCore.Http;
namespace DevJob.Application.ServiceContract
{
    public interface ICVServices
    {
        Task<parseResult> ParceCv(string url, IFormFile cv, string userId,int cvId);
        Task<DeleteCvResult> DeleteCv(string userId, int cvId);
        Task<List<AllCvsDto>?> AllCv(string userId);
        //Task<UploadCvResult> Upload2(string userId, IFormFile file);
        Task<ResponseDto> Upload( string userId,IFormFile file);
    }
}
