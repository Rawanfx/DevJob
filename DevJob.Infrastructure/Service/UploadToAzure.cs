using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DevJob.Application.DTOs.Cvs;
using DevJob.Domain.Entities;
using DevJob.Application.ServiceContract;
using DevJob.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DevJob.Infrastructure.Service
{
    public class UploadToAzure : IUploadToAzure
    {
        private readonly BlobContainerClient blobContainerClient;
        private readonly BlobServiceClient blobServiceClient;
        private IConfiguration configuration;
      //  private readonly ICVServices cVServices;
        private string azureConnectionString;
        private readonly AppDbContext context;
        public UploadToAzure(IConfiguration configuration
           ///ICVServices cVServices
            ,AppDbContext context )
        {
            this.configuration = configuration;
            this.context = context;
           // this.cVServices = cVServices;
            azureConnectionString = configuration["Azure"];
            blobServiceClient = new BlobServiceClient(azureConnectionString);
            blobContainerClient = blobServiceClient.GetBlobContainerClient("cvs");
        }
        public async Task<UploadToAzureResult> UploadFileToAzure(IFormFile file,string userId)
        {
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            using (var stream = file.OpenReadStream())
            {
                await blobContainerClient.UploadBlobAsync(fileName, stream);
            }
            return new UploadToAzureResult() { 
                Link=blobServiceClient.Uri.ToString(),
            FileNameAzure=fileName};
        }
        public async Task Delete(string blobName)
        {
            var blolbClient =  blobContainerClient.GetBlobClient(blobName);
            await blolbClient.DeleteIfExistsAsync();
        }
    }
}
