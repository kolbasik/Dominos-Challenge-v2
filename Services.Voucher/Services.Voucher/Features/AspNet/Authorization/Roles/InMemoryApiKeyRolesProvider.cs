using System;
using System.Collections.Generic;

namespace Services.Voucher.Features.AspNet.Authorization.Roles
{
  public sealed class InMemoryApiKeyRolesProvider : IApiKeyRolesProvider
  {
    public InMemoryApiKeyRolesProvider(IDictionary<string, string[]> roles = null)
    {
      Roles = new Dictionary<string, string[]>(roles ?? new Dictionary<string, string[]>(), StringComparer.Ordinal);
    }

    private Dictionary<string, string[]> Roles { get; }

    public string[] GetRoles(string key)
    {
      return Roles.TryGetValue(key, out var roles) ? roles : Array.Empty<string>();
    }
  }
}
