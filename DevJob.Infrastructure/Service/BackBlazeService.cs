using Amazon.S3;
using Amazon.S3.Model;
using DevJob.Application.ServiceContract;
using Microsoft.Extensions.Configuration;

namespace DevJob.Infrastructure.Service
{
    public class BackBlazeService : IStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public BackBlazeService(IAmazonS3 s3Client, IConfiguration configuration)
        {
            _s3Client = s3Client;
            _bucketName = configuration["MinIO:BucketName"];
        }

        public async Task<bool> DoesFileExistsAsync(string objectKey)
        {
            try
            {
                var metadata = await _s3Client.GetObjectMetadataAsync(_bucketName, objectKey);
                return metadata != null && metadata.ContentLength > 0;
            }
            catch(Exception ex)
            {
                return false;
                throw;
            }
        }

        public string GeneratePresignedUploadUrl(string objectKey)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = objectKey,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(60),
            };
            return _s3Client.GetPreSignedURL(request);
        }

        public string GetCleanVideoUrl(string objectKey)
        {
            return _s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = objectKey,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddMinutes(30)
            });
        }
    }
}