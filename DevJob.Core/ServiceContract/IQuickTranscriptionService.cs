
namespace DevJob.Application.ServiceContract
{
    public interface IQuickTranscriptionService
    {
        Task<string> TranscribeQuickAsync(string storageKey, CancellationToken cancellationToken = default);
    }
}
