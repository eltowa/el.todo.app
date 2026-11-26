namespace Migration;

using Microsoft.Extensions.Logging;

public class Worker(ILogger<Worker> logger) : BackgroundService
{
    private static readonly Action<ILogger, DateTimeOffset, Exception?> _workerRunning =
        LoggerMessage.Define<DateTimeOffset>(
            LogLevel.Information,
            new EventId(1, nameof(Worker)),
            "Worker running at: {Time}");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _workerRunning(logger, DateTimeOffset.Now, null);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
