using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Services.Voucher.Contracts.Models;
using Services.Voucher.Features.Vouchers;
using Xunit;

namespace Services.Voucher.Test.Unit.Repository
{
  public class InMemoryVoucherRepositoryTests
  {
    private readonly List<VoucherModel> _vouchers;
    private readonly InMemoryVoucherRepository _repository;

    public InMemoryVoucherRepositoryTests()
    {
      var fixture = new Fixture();
      _vouchers = fixture.CreateMany<VoucherModel>(1000).ToList();
      _repository = new InMemoryVoucherRepository(_vouchers);
    }

    [Fact]
    public void GetVouchers_ShouldReturnVouchers_UsingSkipAndTake()
    {
      // Arrange
      const int offset = 4;
      const int limit = 15;
      var expected = _vouchers.Skip(offset).Take(limit);

      // Act
      var actual = _repository.GetVouchers(limit, offset);

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetVoucherById_ShouldReturnVoucher_IfFound()
    {
      // Arrange
      var expected = _vouchers.ElementAt(49);

      // Act
      var actual = _repository.GetVoucherById(expected.Id);

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetVoucherById_ShouldReturnNull_IfNotFound()
    {
      // Arrange
      var notExistingId = Guid.NewGuid();

      // Act
      var actual = _repository.GetVoucherById(notExistingId);

      // Assert
      Assert.Null(actual);
    }

    [Fact]
    public void GetVouchersByName_ShouldReturnVouchers_IfFound()
    {
      // Arrange
      var expected = _vouchers.ElementAt(49);

      // Act
      var actual = _repository.GetVouchersByName(expected.Name, 5, 0);

      // Assert
      Assert.Equal(new[] { expected }, actual);
    }

    [Fact]
    public void GetVouchersByName_ShouldReturnEmptyResult_IfNotFound()
    {
      // Arrange
      const string notExistingName = "123";

      // Act
      var actual = _repository.GetVouchersByName(notExistingName, 5, 0);

      // Assert
      Assert.Empty(actual);
    }

    [Fact]
    public void GetCheapestVoucherByProductCode_ShouldReturnCheapestVoucher_IfFound()
    {
      // Arrange
      var expected = _vouchers.ElementAt(49);
      var productCode = expected.ProductCodes.Split(",").First();

      // Act
      var actual = _repository.GetCheapestVoucherByProductCode(productCode);

      // Assert
      Assert.InRange(actual.Price, 0, expected.Price);
    }

    [Fact]
    public void GetCheapestVoucherByProductCode_ShouldReturnNull_IfNotFound()
    {
      // Arrange
      const string notExistingProductCode = "--123--";

      // Act
      var actual = _repository.GetCheapestVoucherByProductCode(notExistingProductCode);

      // Assert
      Assert.Null(actual);
    }
  }
}
