using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using NSubstitute;
using Services.Voucher.Controllers;
using Services.Voucher.Models;
using Services.Voucher.Repository;
using Xunit;

namespace Services.Voucher.Test.Unit.Controllers
{
  public class VoucherControllerTests
  {
    private readonly Fixture _fixture;
    private readonly IVoucherRepository _repository;
    private readonly VoucherController _controller;

    public VoucherControllerTests()
    {
      _fixture = new Fixture();
      _repository = Substitute.For<IVoucherRepository>();
      _controller = new VoucherController(_repository);
    }

    [Fact]
    public void Get_ShouldReturnAllVouchers_WhenNoCountIsProvided()
    {
      // Arrange
      var vouchers = _fixture.CreateMany<VoucherModel>(1000);
      _repository.GetVouchers().Returns(vouchers);

      // Act
      var result = _controller.Get();

      // Assert
      Assert.Equal(vouchers.Count(), result.Count());
    }

    [Fact]
    public void Get_ShouldReturnTheRequestedAmountOfVouchers_WhenACountIsProvided()
    {
      // Arrange
      var count = 5;
      var vouchers = _fixture.CreateMany<VoucherModel>(1000);
      _repository.GetVouchers().Returns(vouchers);

      // Act
      var result = _controller.Get(count);

      // Assert
      Assert.Equal(count, result.Count());
    }

    [Fact]
    public void GetVoucherById_StateUnderTest_ExpectedBehavior()
    {
      // Arrange
      var vouchers = _fixture.CreateMany<VoucherModel>(5);
      _repository.GetVouchers().Returns(vouchers);
      var expected = vouchers.ElementAt(2);

      // Act
      var result = _controller.GetVoucherById(expected.Id);

      // Assert
      Assert.Equal(result, expected);
    }

    [Fact]
    public void GetVouchersByName_ShouldReturnAllVouchersWithTheGivenSearchString_WhenVoucherExists()
    {
      // Arrange
      var vouchers = new List<VoucherModel>
      {
        new VoucherModel { Id = Guid.NewGuid(), Name = "A" },
        new VoucherModel { Id =  Guid.NewGuid(), Name =  "B" },
        new VoucherModel { Id =  Guid.NewGuid(), Name =  "A" }
      };
      _repository.GetVouchers().Returns(vouchers);

      // Act
      var result = _controller.GetVouchersByName("A");

      // Assert
      Assert.Equal(result, new List<VoucherModel> { vouchers.ElementAt(0), vouchers.ElementAt(2) });
    }

    [Fact]
    public void GetVouchersByNameSearch_ShouldReturnAllVouchersThatContainTheGivenSearchString_WhenVoucherExists()
    {
      // Arrange
      var vouchers = new List<VoucherModel>
      {
        new VoucherModel { Id =  Guid.NewGuid(), Name =  "ABC" },
        new VoucherModel { Id =  Guid.NewGuid(), Name =  "ABCD" },
        new VoucherModel { Id =  Guid.NewGuid(), Name =  "ACD" }
      };
      _repository.GetVouchers().Returns(vouchers);

      // Act
      var result = _controller.GetVouchersByNameSearch("BC");

      // Assert
      Assert.Equal(result, new List<VoucherModel> { vouchers.ElementAt(0), vouchers.ElementAt(1) });
    }

    [Fact]
    public void GetCheapestVoucherByProductCode_StateUnderTest_ExpectedBehavior()
    {
      // Arrange
      var vouchers = new List<VoucherModel>
      {
        new VoucherModel { Id =  Guid.NewGuid(), ProductCodes = "QWE", Price = 789 },
        new VoucherModel { Id =  Guid.NewGuid(), ProductCodes = "QWE", Price = 123 },
        new VoucherModel { Id =  Guid.NewGuid(), ProductCodes = "QWE", Price = 456 }
      };
      _repository.GetVouchers().Returns(vouchers);
      var expected = vouchers.ElementAt(1);

      // Act
      var result = _controller.GetCheapestVoucherByProductCode("QWE");

      // Assert
      Assert.Equal(result, expected);
    }

    // TODO: This is not all the tests that we would like to see + the above tests can be made much smarter.
  }
}
