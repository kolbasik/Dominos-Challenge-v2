using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Voucher.Contracts;
using Services.Voucher.Features.ServiceAccessors;

namespace Services.Voucher.Features.Vouchers
{
  public sealed class UpdateInMemoryVouchersBackgroundService : BackgroundService
  {
    public UpdateInMemoryVouchersBackgroundService(
      IOptions<UpdateInMemoryVouchersBackgroundOptions> options,
      ILogger<UpdateInMemoryVouchersBackgroundService> logger,
      InMemoryVouchersHealthCheck healthCheck,
      IVoucherProvider voucherProvider,
      ServiceAccessor<IVoucherRepository> voucherRepositoryAccessor,
      ServiceAccessor<IVoucherSearch> voucherSearchAccessor
    )
    {
      BackgroundOptions = Ensure.NotNull(options, nameof(options)).Value;
      Logger = Ensure.NotNull(logger, nameof(logger));
      HealthCheck = Ensure.NotNull(healthCheck, nameof(healthCheck));
      VoucherProvider = Ensure.NotNull(voucherProvider, nameof(voucherProvider));
      VoucherRepositoryAccessor = Ensure.NotNull(voucherRepositoryAccessor, nameof(voucherRepositoryAccessor));
      VoucherSearchAccessor = Ensure.NotNull(voucherSearchAccessor, nameof(voucherSearchAccessor));
    }

    private UpdateInMemoryVouchersBackgroundOptions BackgroundOptions { get; }
    private ILogger Logger { get; }
    private InMemoryVouchersHealthCheck HealthCheck { get; }
    private IVoucherProvider VoucherProvider { get; }
    private ServiceAccessor<IVoucherRepository> VoucherRepositoryAccessor { get; }
    private ServiceAccessor<IVoucherSearch> VoucherSearchAccessor { get; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      var healthStatusOnError = HealthStatus.Unhealthy;
      while (!stoppingToken.IsCancellationRequested)
      {
        try
        {
          var vouchers = VoucherProvider.GetVouchers().ToList();
          VoucherRepositoryAccessor.Current = new InMemoryVoucherRepository(vouchers);
          VoucherSearchAccessor.Current = BackgroundOptions.SearchProvider switch
          {
            SearchProviders.Trigrams => new InMemoryTrigramVoucherSearch(vouchers),
            _ => new InMemoryLuceneVoucherSearch(vouchers)
          };
          HealthCheck.Status = HealthStatus.Healthy;
          healthStatusOnError = HealthStatus.Degraded;
          await Task.Delay(BackgroundOptions.UpdateInterval, stoppingToken);
        }
        catch (Exception error)
        {
          if (stoppingToken.IsCancellationRequested)
          {
            return;
          }
          HealthCheck.Status = healthStatusOnError;
          Logger.LogError(error, "Unable to update the vouchers in the memory");
          await Task.Delay(BackgroundOptions.RetryOnFailure, stoppingToken);
        }
      }
    }
  }
}
