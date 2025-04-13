using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChatApp.Application.Services;

public class BackgroundTaskRunner : IHostedService
{
    private readonly IBackgroundTaskQueue _backgroundTaskQueue;
    private readonly ILogger<BackgroundTaskRunner> _logger;

    public BackgroundTaskRunner(IBackgroundTaskQueue backgroundTaskQueue, ILogger<BackgroundTaskRunner> logger)
    {
        _backgroundTaskQueue = backgroundTaskQueue;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(ProcessQueueAsync);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task ProcessQueueAsync()
    {
        // TODO: Cancellation token support
        while (true)
        {
            Func<Task>? task = _backgroundTaskQueue.Dequeue();
            if (task is not null)
            {
                _logger.LogDebug("Executing a background task");
                Task.Run(async () =>
                {
                    try
                    {
                        await task();
                        _logger.LogDebug("Background task executed");
                    }

                    catch (Exception ex)
                    {
                        _logger.LogError(ex.ToString());
                    }
                });
            }
            else
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500));
            }
        }
    }
}