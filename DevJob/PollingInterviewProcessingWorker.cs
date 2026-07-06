using DevJob.Application.ServiceContract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
namespace DevJob.API
{
    public class PollingInterviewProcessingWorker : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<PollingInterviewProcessingWorker> logger;
        private readonly TimeSpan pollInterval;
        private readonly int batchSize;

        public PollingInterviewProcessingWorker( 
            IServiceScopeFactory scopeFactory,
            ILogger<PollingInterviewProcessingWorker> logger,
            TimeSpan? pollInterval = null,
            int batchSize = 5)
        {
            this.scopeFactory = scopeFactory;
            this.logger = logger;
            this.pollInterval = pollInterval ?? TimeSpan.FromSeconds(15);
            this.batchSize = batchSize;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(pollInterval);
            await ProcessQueuedVideosAsync(stoppingToken); 

            while (!stoppingToken.IsCancellationRequested
                   && await timer.WaitForNextTickAsync(stoppingToken))
            {
                await ProcessQueuedVideosAsync(stoppingToken);
            }
        }

        private async Task ProcessQueuedVideosAsync(CancellationToken stoppingToken)
        {
            using var scope = scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var processor = scope.ServiceProvider.GetRequiredService<IInterviewProcessService>();

            var queuedVideos = await unitOfWork.InterviewVideo.GetByStatusAsync(VideoStatus.Queued, batchSize);

            foreach (var video in queuedVideos)
            {
                if (stoppingToken.IsCancellationRequested) break;
                try
                {
                    await processor.ProcessJobAsync(video.Id, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to process interview video {VideoId}.", video.Id);
                    video.Status = VideoStatus.Failed;
                    video.ErrorMessage = ex.Message;
                    await unitOfWork.SaveChangesAsync();
                }
            }
        }
    }
}
