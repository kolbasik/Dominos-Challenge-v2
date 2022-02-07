using System.Linq;
using AutoFixture;
using Services.Voucher.Contracts.Models;
using Services.Voucher.Features.Vouchers;
using Xunit;

namespace Services.Voucher.Test.Unit.Features.Vouchers
{
  public class InMemoryTrigramVoucherSearchTests
  {
    private readonly VoucherModel _voucher;
    private readonly InMemoryTrigramVoucherSearch _trigram;

    public InMemoryTrigramVoucherSearchTests()
    {
      var fixture = new Fixture();
      _voucher = fixture.Create<VoucherModel>() with { Name = "Hi, I like a cake." };
      var vouchers = fixture.CreateMany<VoucherModel>(1000).ToList();
      vouchers.Add(_voucher);
      _trigram = new InMemoryTrigramVoucherSearch(vouchers);
    }

    [Fact]
    public void GetVouchers_ShouldReturnVouchers_UsingSkipAndTake()
    {
      // Arrange
      var patterns = new[] { "hi cake", "like cake", "hi like" };

      // Act
      var actual = patterns.Select(term => _trigram.Search(term, 1).First());

      // Assert
      Assert.InRange(actual.Count(), patterns.Length, patterns.Length);
      Assert.Equal(new[] { _voucher }, actual.Distinct());
    }
  }
}
