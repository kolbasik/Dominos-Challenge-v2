using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Services.Voucher.HealthChecks.Startup
{
  internal sealed class StartupHealthCheckBackgroundService : BackgroundService
  {
    private readonly StartupHealthCheck _healthCheck;

    public StartupHealthCheckBackgroundService(StartupHealthCheck healthCheck)
      => _healthCheck = healthCheck;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
      _healthCheck.StartupCompleted = true;
      return Task.CompletedTask;
    }
  }
}
