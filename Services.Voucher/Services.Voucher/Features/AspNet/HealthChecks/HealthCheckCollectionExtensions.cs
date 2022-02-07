using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Services.Voucher.Features.AspNet.HealthChecks.Startup;

namespace Services.Voucher.Features.AspNet.HealthChecks
{
  /// <summary>
  /// Register a startup health check to indicate that the application is running.
  /// <see cref="https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-6.0"/>
  /// </summary>
  [ExcludeFromCodeCoverage]
  public static class HealthCheckCollectionExtensions
  {
    public static IHealthChecksBuilder AddStartupCheck(this IHealthChecksBuilder builder)
    {
      builder.Services.AddSingleton<StartupHealthCheck>();
      builder.Services.AddHostedService<StartupHealthCheckBackgroundService>();
      builder.AddCheck<StartupHealthCheck>("Startup", tags: new[] { "ready" });
      return builder;
    }
  }
}
