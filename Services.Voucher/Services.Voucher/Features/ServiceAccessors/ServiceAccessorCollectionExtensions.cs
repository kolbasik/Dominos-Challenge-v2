using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Services.Voucher.Features.ServiceAccessors
{
  [ExcludeFromCodeCoverage]
  public static class ServiceAccessorCollectionExtensions
  {
    public static ServiceAccessor<TService> AddServiceAccessor<TService>(this IServiceCollection services)
      where TService : class
    {
      var serviceAccessor = new ServiceAccessor<TService>();
      services.AddSingleton(serviceAccessor);
      services.AddSingleton<IServiceAccessor<TService>>(serviceAccessor);
      services.AddTransient(_ => serviceAccessor.Current);
      return serviceAccessor;
    }
  }
}
