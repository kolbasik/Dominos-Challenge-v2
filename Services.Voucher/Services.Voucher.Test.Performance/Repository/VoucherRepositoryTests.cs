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
  public class VoucherRepositoryTests
  {
    private readonly IVoucherRepository _repository;

    public VoucherRepositoryTests()
    {
      var fixture = new Fixture();
      var random = new Random(Guid.NewGuid().GetHashCode());
      fixture.Customize<VoucherModel>(ctx =>
        ctx.WithAutoProperties()
          .With(x => x.ProductCodes,
            () => string.Join(",", Enumerable.Range(0, 3).Select(_ => random.Next(100, 999).ToString())))
      );
      _repository = new InMemoryVoucherRepository(fixture.CreateMany<VoucherModel>(1000).ToList());
    }

    [Fact]
    public void GetVouchers_ShouldBePerformant()
    {
      var time = Stopwatch.StartNew();
      for (var i = 0; i < 1000; i++) _repository.GetVouchers(50, 200);
      Assert.InRange(time.Elapsed.TotalMilliseconds, 0, 100);
    }

    [Fact]
    public void GetVoucherById_ShouldBePerformant()
    {
      var expected = _repository.GetVouchers(450, 1).First();
      var time = Stopwatch.StartNew();
      for (var i = 0; i < 1000; i++) _repository.GetVoucherById(expected.Id);
      Assert.InRange(time.Elapsed.TotalMilliseconds, 0, 100);
    }

    [Fact]
    public void GetVoucherByName_ShouldBePerformant()
    {
      var expected = _repository.GetVouchers(450, 1).First();
      var time = Stopwatch.StartNew();
      for (var i = 0; i < 1000; i++) _repository.GetVouchersByName(expected.Name, 50, 0);
      Assert.InRange(time.Elapsed.TotalMilliseconds, 0, 100);
    }

    [Fact]
    public void GetCheapestVoucherByProductCode_ShouldBePerformant()
    {
      var expected = _repository.GetVouchers(450, 1).First();
      var time = Stopwatch.StartNew();
      for (var i = 0; i < 1000; i++)
        _repository.GetCheapestVoucherByProductCode(expected.ProductCodes.Split(",").First());
      Assert.InRange(time.Elapsed.TotalMilliseconds, 0, 100);
    }
  }
}
