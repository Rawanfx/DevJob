using Amazon.S3;
using DevJob.Application.DTOs.MockInterview;
using DevJob.Application.ServiceContract;
using DevJob.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Runtime;
using System.Security.Cryptography.Xml;
namespace DevJob.Infrastructure.Service
{
    public class QuickTranscriptionService : IQuickTranscriptionService
    {
        private readonly HttpClient _httpClient;
        private readonly IStorageService storageService;
        private readonly string _fastApiBaseUrl;
        public QuickTranscriptionService(HttpClient httpClient,IStorageService storageService, IOptions<FastApiSettings> options)
        {
            this._httpClient = httpClient;
            this.storageService = storageService;
            _fastApiBaseUrl = options.Value.BaseUrl;
        }
        public async Task<string> TranscribeQuickAsync(string storageKey, CancellationToken cancellationToken = default)
        {
            var dowenloadUrl = storageService.GetCleanVideoUrl(storageKey);
            var response = await _httpClient.PostAsJsonAsync(
           $"{_fastApiBaseUrl}/transcribe",
           new TranscribeRequest { VideoUrl = dowenloadUrl },
           cancellationToken);

            response.EnsureSuccessStatusCode();
            var result = await response.Content
                     .ReadFromJsonAsync<TranscribeResponse>(
                         cancellationToken: cancellationToken);

            if (result is null || !result.Success)
                throw new InvalidOperationException(
                    $"Transcription failed: {result?.Message ?? "null response"}");


            return result.Text;
        }
    }
}
