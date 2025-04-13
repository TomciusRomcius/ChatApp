using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChatApp.Application.Services;

public class IOBackgroundRunner : IHostedService
{
    private readonly IBackgroundTaskQueue _backgroundTaskQueue;
    private readonly ILogger<IOBackgroundRunner> _logger;

    public IOBackgroundRunner(IBackgroundTaskQueue backgroundTaskQueue, ILogger<IOBackgroundRunner> logger)
    {
        _backgroundTaskQueue = backgroundTaskQueue;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(ProcessQueueAsync, CancellationToken.None).ConfigureAwait(false);
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
            Func<Task>? func = _backgroundTaskQueue.Dequeue();
            if (func is not null)
            {
                // TODO: store tasks and log errors
                _logger.LogDebug("Executing a background task");
                var task = func();
            }
            else
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500));
            }
        }
    }
}