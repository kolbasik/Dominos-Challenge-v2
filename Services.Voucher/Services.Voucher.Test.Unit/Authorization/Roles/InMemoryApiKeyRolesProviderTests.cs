using System;
using System.Linq;
using AutoFixture;
using J2N.Collections.Generic;
using Services.Voucher.Features.AspNet.Authorization.Roles;
using Xunit;

namespace Services.Voucher.Test.Unit.Authorization.Roles
{
  public class InMemoryApiKeyRolesProviderTests
  {
    private readonly Fixture _fixture;

    public InMemoryApiKeyRolesProviderTests()
    {
      _fixture = new Fixture();
    }

    [Fact]
    public void GetRoles_ShouldReturnNoneRoles_IfUnknown()
    {
      // Arrange
      var provider = new InMemoryApiKeyRolesProvider();
      var expected = Array.Empty<string>();

      // Act
      var actual = provider.GetRoles("UNKNOWN");

      // Assert
      Assert.Equal(expected, actual);
    }


    [Fact]
    public void GetRoles_ShouldReturnRoles_IfKnown()
    {
      // Arrange
      var key = _fixture.Create<string>();
      var expected = _fixture.CreateMany<string>().ToArray();
      var provider = new InMemoryApiKeyRolesProvider(new Dictionary<string, string[]>
      {
        [key] = expected
      });

      // Act
      var actual = provider.GetRoles(key);

      // Assert
      Assert.Equal(expected, actual);
    }
  }
}
