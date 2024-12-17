using SampleCron.Jobs;

namespace SampleCron;

public class Worker : BackgroundService
{
    public Worker()
    {
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var time = DateTime.UtcNow;

            await SendPostJob.ExecuteAsync(time);

            await Task.Delay(1000, stoppingToken);
        }
    }
}