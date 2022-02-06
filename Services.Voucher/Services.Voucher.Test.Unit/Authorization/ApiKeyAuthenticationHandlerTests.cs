using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using NSubstitute;
using Services.Voucher.Authorization;
using Services.Voucher.Authorization.Roles;
using Xunit;

namespace Services.Voucher.Test.Unit.Authorization
{
  public class ApiKeyAuthenticationHandlerTests
  {
    private readonly Fixture _fixture;
    private readonly IApiKeyRolesProvider _provider;
    private readonly ApiKeyAuthenticationHandler _handler;

    public ApiKeyAuthenticationHandlerTests()
    {
      _fixture = new Fixture();
      _provider = Substitute.For<IApiKeyRolesProvider>();
      _handler = new ApiKeyAuthenticationHandler(_provider);
    }

    [Fact]
    public async Task ChallengeAsync_ShouldReturn401()
    {
      // Arrange
      var scheme = new AuthenticationScheme("TEST", "TEST", Substitute.For<IAuthenticationHandler>().GetType());
      var context = new DefaultHttpContext(Substitute.For<IFeatureCollection>());
      await _handler.InitializeAsync(scheme, context);

      // Act
      await _handler.ChallengeAsync(new AuthenticationProperties());

      // Assert
      Assert.Equal(401, context.Response.StatusCode);
    }

    [Fact]
    public async Task ForbidAsync_ShouldReturn403()
    {
      // Arrange
      var scheme = new AuthenticationScheme("TEST", "TEST", Substitute.For<IAuthenticationHandler>().GetType());
      var context = new DefaultHttpContext(Substitute.For<IFeatureCollection>());
      await _handler.InitializeAsync(scheme, context);

      // Act
      await _handler.ForbidAsync(new AuthenticationProperties());

      // Assert
      Assert.Equal(403, context.Response.StatusCode);
    }

    [Fact]
    public async Task AuthenticateAsync_AuthenticateResultNoResult_IfNoApiKey()
    {
      // Arrange
      var scheme = new AuthenticationScheme("TEST", "TEST", Substitute.For<IAuthenticationHandler>().GetType());
      var context = new DefaultHttpContext(Substitute.For<IFeatureCollection>());
      await _handler.InitializeAsync(scheme, context);

      // Act
      var actual = await _handler.AuthenticateAsync();

      // Assert
      Assert.True(actual.None);
    }

    [Fact]
    public async Task AuthenticateAsync_AuthenticateResultSuccess_IfApiKeyIsDefined()
    {
      // Arrange
      var schemeName = _fixture.Create<string>();
      var key = _fixture.Create<string>();
      var roles = _fixture.CreateMany<string>().ToArray();
      var features = Substitute.For<IFeatureCollection>();
      var scheme = new AuthenticationScheme(schemeName, schemeName, Substitute.For<IAuthenticationHandler>().GetType());
      var context = new DefaultHttpContext(features);
      context.Request.Headers.Returns(new HeaderDictionary());
      context.Request.Headers.Add(ApiKeys.HeaderName, key);

      _provider.GetRoles(key).Returns(roles);

      await _handler.InitializeAsync(scheme, context);

      // Act
      var actual = await _handler.AuthenticateAsync();

      // Assert
      Assert.False(actual.None);
      Assert.NotNull(actual.Principal?.Identity);
      Assert.True(actual.Principal.Identity.IsAuthenticated);
      Assert.Equal(schemeName, actual.Principal.Identity.AuthenticationType);
      Assert.True(actual.Principal.IsInRole(roles.ElementAt(1)));
      Assert.False(actual.Principal.IsInRole("UNKNOWN"));
    }
  }
}
