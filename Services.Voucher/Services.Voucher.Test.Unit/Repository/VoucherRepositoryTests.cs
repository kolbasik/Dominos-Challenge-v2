using System.Linq;
using AutoFixture;
using Services.Voucher.Models;
using Services.Voucher.Repository;
using Xunit;

namespace Services.Voucher.Test.Unit.Repository
{
  public class VoucherRepositoryTests
  {
    private readonly Fixture _fixture;

    public VoucherRepositoryTests()
    {
      _fixture = new Fixture();
    }

    [Fact]
    public void Get_ShouldReturnAllVouchers_WhenNoCountIsProvided()
    {
      // Arrange
      var vouchers = _fixture.CreateMany<VoucherModel>(1000);
      var repository = new VoucherRepository(vouchers);

      // Act
      var result = repository.GetVouchers();

      // Assert
      Assert.Equal(vouchers, result);
    }
  }
}
