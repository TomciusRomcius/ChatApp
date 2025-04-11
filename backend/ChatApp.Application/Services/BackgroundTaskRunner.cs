using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChatApp.Application.Services
{
    public class BackgroundTaskRunner : IHostedService
    {
        readonly IBackgroundTaskQueue _backgroundTaskQueue;
        readonly ILogger<BackgroundTaskRunner> _logger;

        public BackgroundTaskRunner(IBackgroundTaskQueue backgroundTaskQueue, ILogger<BackgroundTaskRunner> logger)
        {
            _backgroundTaskQueue = backgroundTaskQueue;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // TODO: probably dont have to spawn a separate thread, cant find any info
            Task.Run(ProcessQueueAsync);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task ProcessQueueAsync()
        {
            while (true)
            {
                try
                {
                    Func<Task>? task = _backgroundTaskQueue.Dequeue();
                    if (task is not null)
                    {
                        _logger.LogDebug("Executing a background task");
                        await task();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }
        }
    }
}