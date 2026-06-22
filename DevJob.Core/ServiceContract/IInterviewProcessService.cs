namespace DevJob.Application.ServiceContract
{
    public interface IInterviewProcessService
    {
        Task ProcessJobAsync(Guid videoId, CancellationToken cancellationToken);
    }
}
