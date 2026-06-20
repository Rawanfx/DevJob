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
            var url = _s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = objectKey,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddDays(7)
            });
            return url.Split('?')[0];
        }
    }
}