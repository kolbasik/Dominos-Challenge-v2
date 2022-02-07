using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Services.Voucher.Features.AspNet.Authorization.Roles;

namespace Services.Voucher.Features.AspNet.Authorization
{
  public sealed class ApiKeyAuthenticationHandler : IAuthenticationHandler
  {
    private readonly IApiKeyRolesProvider _rolesProvider;
    private AuthenticationScheme _scheme;
    private HttpContext _context;

    public ApiKeyAuthenticationHandler(IApiKeyRolesProvider rolesProvider)
    {
      _rolesProvider = Ensure.NotNull(rolesProvider, nameof(rolesProvider));
    }

    public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
    {
      _scheme = scheme;
      _context = context;
      return Task.CompletedTask;
    }

    public Task ChallengeAsync(AuthenticationProperties properties)
    {
      _context.Response.StatusCode = 401;
      return Task.CompletedTask;
    }

    public Task ForbidAsync(AuthenticationProperties properties)
    {
      _context.Response.StatusCode = 403;
      return Task.CompletedTask;
    }

#pragma warning disable CS1998
    public async Task<AuthenticateResult> AuthenticateAsync()
#pragma warning restore CS1998
    {
      if (_context.Request.Headers.TryGetValue(ApiKeys.HeaderName, out var keyHeader))
      {
        var key = keyHeader[0];
        var claims = new List<Claim> { new (ClaimTypes.NameIdentifier, key) };
        var roles = _rolesProvider.GetRoles(key);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, _scheme.Name));
        return AuthenticateResult.Success(new AuthenticationTicket(principal, authenticationScheme: _scheme.Name));
      }
      return AuthenticateResult.NoResult();
    }
  }
}
