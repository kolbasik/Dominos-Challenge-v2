using System;
using System.Diagnostics;
using System.Linq;
using AutoFixture;
using Services.Voucher.Contracts;
using Services.Voucher.Contracts.Models;
using Services.Voucher.Features.Vouchers;
using Xunit;

namespace Services.Voucher.Test.Performance.Repository
{
  public class LuceneVoucherSearchTests
  {
    private readonly IVoucherSearch _search;

    public LuceneVoucherSearchTests()
    {
      var fixture = new Fixture();
      var random = new Random(Guid.NewGuid().GetHashCode());
      fixture.Customize<VoucherModel>(ctx =>
        ctx.WithAutoProperties()
          .With(x => x.ProductCodes,
            () => string.Join(",", Enumerable.Range(0, 3).Select(_ => random.Next(100, 999).ToString())))
      );
      var vouchers = fixture.CreateMany<VoucherModel>(1000).ToList();
      vouchers.Add(fixture.Create<VoucherModel>() with { Name = "Lorem ipsum dolor sit amet, consectetur adipiscing elit." });
      _search = new InMemoryLuceneVoucherSearch(vouchers);
    }

    [Fact]
    public void Search_ShouldBePerformant()
    {
      var time = Stopwatch.StartNew();
      for (var i = 0; i < 1000; i++) _search.Search("Lorem adipiscing", 50);
      Assert.InRange(time.Elapsed.TotalMilliseconds, 0, 1000);
    }
  }
}
