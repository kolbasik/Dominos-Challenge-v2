using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Services.Voucher.Contracts.Models;
using Services.Voucher.Features;
using Xunit;

namespace Services.Voucher.Test.Unit.Features
{
  public class PaginationTests
  {
    private readonly List<VoucherModel> _vouchers;

    public PaginationTests()
    {
      _vouchers = new Fixture().CreateMany<VoucherModel>(1000).ToList();
    }

    [Fact]
    public void Paginate_ShouldLimitItems_UsingCount()
    {
      // Arrange
      const int limit = 23;
      const int offset = 0;
      var expected = _vouchers.Skip(offset).Take(limit);

      // Act
      var actual = _vouchers.Paginate(limit, offset);

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void Paginate_ShouldSkipItems_UsingOffset()
    {
      // Arrange
      const int limit = 21;
      const int offset = 17;
      var expected = _vouchers.Skip(offset).Take(limit);

      // Act
      var actual = _vouchers.Paginate(limit, offset);

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void Paginate_ShouldIgnoreOffset_IfOffsetIsNegative()
    {
      // Arrange
      const int limit = 21;
      var expected = _vouchers.Skip(0).Take(limit);

      // Act
      var actual = _vouchers.Paginate(limit, -25);

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void Paginate_ShouldReturnAtLeastOneItem_IfCountIsLessThanOne()
    {
      // Arrange
      const int offset = 17;
      var expected = _vouchers.Skip(offset).Take(1);

      // Act
      var actual = _vouchers.Paginate(-1, offset);

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void Paginate_ShouldReturnMaximumAllowedAmountOfItems_IfCountIsMoreThanMaxValue()
    {
      // Arrange
      const int offset = 17;
      var expected = _vouchers.Skip(offset).Take(Pagination.MaxValue);

      // Act
      var actual = _vouchers.Paginate(Pagination.MaxValue + 1, offset);

      // Assert
      Assert.Equal(expected, actual);
    }
  }
}
