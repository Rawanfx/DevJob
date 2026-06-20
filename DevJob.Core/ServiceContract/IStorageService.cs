using DevJob.Application.DTOs.MockInterview;
using Microsoft.AspNetCore.Http;

namespace DevJob.Application.ServiceContract
{
    public interface IStorageService
    {
        string GetCleanVideoUrl(string objectKey);
        string GeneratePresignedUploadUrl(string objectKey);
    }
}
