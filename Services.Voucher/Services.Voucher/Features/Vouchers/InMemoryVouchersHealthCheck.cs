using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Services.Voucher.Features.Vouchers
{
  [ExcludeFromCodeCoverage]
  public sealed class InMemoryVouchersHealthCheck : IHealthCheck
  {
    private volatile HealthStatus _status;

    public HealthStatus Status
    {
      get => _status;
      set => _status = value;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
      switch (Status)
      {
        case HealthStatus.Healthy:
          return Task.FromResult(HealthCheckResult.Healthy("The vouchers has been loaded in the memory."));
        case HealthStatus.Degraded:
          return Task.FromResult(HealthCheckResult.Degraded("The vouchers are not up-to-date."));
        case HealthStatus.Unhealthy:
        default:
          return Task.FromResult(HealthCheckResult.Unhealthy("The vouchers are not loaded yet."));
      }
    }
  }
}
