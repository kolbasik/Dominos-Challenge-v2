using System;
using AutoFixture;
using NSubstitute;
using Services.Voucher.Controllers;
using Services.Voucher.Models;
using Services.Voucher.Repository;
using Xunit;

namespace Services.Voucher.Test.Performance.Controllers
{
  public class VoucherControllerTests
  {
    private readonly VoucherController _controller;

    public VoucherControllerTests()
    {
      var fixture = new Fixture();
      fixture.Customize<VoucherModel>(it => it.WithAutoProperties().With(o => o.ProductCodes, "P007D"));
      var repository = Substitute.For<IVoucherRepository>();
      repository.GetVouchers().Returns(fixture.CreateMany<VoucherModel>(1000));
      _controller = new VoucherController(repository);
    }

    [Fact]
    public void Get_ShouldBePerformant()
    {
      var startTime = DateTime.Now;

      for (var i = 0; i < 1000; i++) _controller.Get();

      var elapsed = DateTime.Now.Subtract(startTime).TotalMilliseconds;
      Assert.True(elapsed < 15000);
    }

    [Fact]
    public void Get_ShouldBePerformantWhenReturningASubset()
    {
      var startTime = DateTime.Now;

      for (var i = 0; i < 100000; i++) _controller.Get(1000);

      var elapsed = DateTime.Now.Subtract(startTime).TotalMilliseconds;
      Assert.True(elapsed < 5000);
    }

    [Fact]
    public void GetCheapestVoucherByProductCode_ShouldBePerformant()
    {
      var startTime = DateTime.Now;

      for (var i = 0; i < 100; i++) _controller.GetCheapestVoucherByProductCode("P007D");

      var elapsed = DateTime.Now.Subtract(startTime).TotalMilliseconds;
      Assert.True(elapsed < 15000);
    }

    // TODO: This is not all the tests that we would like to see + the above tests can be made much smarter.
  }
}
