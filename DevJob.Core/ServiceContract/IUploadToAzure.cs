using Azure.Storage.Blobs.Models;
using DevJob.Application.DTOs.Cvs;
using Microsoft.AspNetCore.Http;
namespace DevJob.Application.ServiceContract
{
    public interface IUploadToAzure
    {
        Task<UploadToAzureResult> UploadFileToAzure(IFormFile file, string userId);
        Task Delete(string blobName);
    }
}
