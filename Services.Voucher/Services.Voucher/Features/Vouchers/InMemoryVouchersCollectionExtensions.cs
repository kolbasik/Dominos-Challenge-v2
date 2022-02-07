using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Services.Voucher.Contracts;
using Services.Voucher.Features.ServiceAccessors;

namespace Services.Voucher.Features.Vouchers
{
  [ExcludeFromCodeCoverage]
  public static class InMemoryVouchersCollectionExtensions
  {
    public static IServiceCollection AddInMemoryVouchers(this IServiceCollection services, Action<UpdateInMemoryVouchersBackgroundOptions> configureOptions = null)
    {
      services.AddServiceAccessor<IVoucherRepository>();
      services.AddServiceAccessor<IVoucherSearch>();
      services.AddOptions<UpdateInMemoryVouchersBackgroundOptions>();
      if (configureOptions != null)
      {
        services.Configure(configureOptions);
      }
      services.AddSingleton<InMemoryVouchersHealthCheck>();
      services.AddHealthChecks().AddCheck<InMemoryVouchersHealthCheck>("InMemoryVouchers", tags: new[] { "ready" });
      services.AddHostedService<UpdateInMemoryVouchersBackgroundService>();
      return services;
    }
  }
}
