using System.Linq;
using AutoFixture;
using Services.Voucher.Contracts.Models;
using Services.Voucher.Features.Vouchers;
using Xunit;

namespace Services.Voucher.Test.Unit.Features.Vouchers
{
  public class InMemoryLuceneVoucherSearchTests
  {
    private readonly VoucherModel _voucher;
    private readonly InMemoryLuceneVoucherSearch _lucene;

    public InMemoryLuceneVoucherSearchTests()
    {
      var fixture = new Fixture();
      _voucher = fixture.Create<VoucherModel>() with { Name = "Hi, I like a cake." };
      var vouchers = fixture.CreateMany<VoucherModel>(1000).ToList();
      vouchers.Add(_voucher);
      _lucene = new InMemoryLuceneVoucherSearch(vouchers);
    }

    [Fact]
    public void GetVouchers_ShouldReturnVouchers_UsingSkipAndTake()
    {
      // Arrange
      var patterns = new[] { "hi cake", "like cake", "hi like" };

      // Act
      var actual = patterns.Select(term => _lucene.Search(term, 1).First());

      // Assert
      Assert.InRange(actual.Count(), patterns.Length, patterns.Length);
      Assert.Equal(new[] { _voucher }, actual.Distinct());
    }
  }
}
