using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Services.Voucher.Features.AspNet.Authorization.Roles;

namespace Services.Voucher.Features.AspNet.Authorization
{
  [ExcludeFromCodeCoverage]
  public static class ApiKeyCollectionExtensions
  {
    public static IServiceCollection AddInMemoryApiKeyRolesProvider(this IServiceCollection services, IDictionary<string, string[]> roles = null)
    {
      services.AddSingleton<IApiKeyRolesProvider>(new InMemoryApiKeyRolesProvider(roles));
      return services;
    }
  }
}
