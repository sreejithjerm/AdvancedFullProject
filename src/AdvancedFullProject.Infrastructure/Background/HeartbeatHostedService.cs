using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AdvancedFullProject.Infrastructure.Background;

public sealed class HeartbeatHostedService : BackgroundService
{
    private readonly ILogger<HeartbeatHostedService> _logger;
    public HeartbeatHostedService(ILogger<HeartbeatHostedService> logger) => _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Heartbeat worker running at {Time}", DateTimeOffset.UtcNow);
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
