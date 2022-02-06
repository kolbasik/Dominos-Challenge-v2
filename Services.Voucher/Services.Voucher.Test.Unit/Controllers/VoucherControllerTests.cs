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
    private readonly IVoucherSearch _search;
    private readonly VoucherController _controller;

    public VoucherControllerTests()
    {
      _fixture = new Fixture();
      _repository = Substitute.For<IVoucherRepository>();
      _search = Substitute.For<IVoucherSearch>();
      _controller = new VoucherController(_repository, _search);
    }

    [Fact]
    public void Get_ShouldReturnAllVouchers_WhenNoCountIsProvided()
    {
      // Arrange
      var expected = _fixture.CreateMany<VoucherModel>(1000);
      _repository.GetVouchers(25, 0).Returns(expected);

      // Act
      var actual = _controller.Get();

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void Get_ShouldReturnTheRequestedAmountOfVouchers_WhenACountIsProvided()
    {
      // Arrange
      const int offset = 3;
      const int limit = 5;
      var expected = _fixture.CreateMany<VoucherModel>(1000);
      _repository.GetVouchers(limit, offset).Returns(expected);

      // Act
      var actual = _controller.Get(limit, offset);

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetVoucherById_StateUnderTest_ExpectedBehavior()
    {
      // Arrange
      var expected = _fixture.Create<VoucherModel>();
      _repository.GetVoucherById(expected.Id).Returns(expected);

      // Act
      var result = _controller.GetVoucherById(expected.Id);

      // Assert
      Assert.Equal(expected, result);
    }

    [Fact]
    public void GetVouchersByName_ShouldReturnAllVouchersWithTheGivenSearchString_WhenVoucherExists()
    {
      // Arrange
      var expected = _fixture.CreateMany<VoucherModel>(5);
      _repository.GetVouchersByName("BC", 25, 0).Returns(expected);

      // Act
      var actual = _controller.GetVouchersByName("BC");

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetVouchersByNameSearch_ShouldReturnAllVouchersThatContainTheGivenSearchString_WhenVoucherExists()
    {
      // Arrange
      var expected = _fixture.CreateMany<VoucherModel>(5);
      _search.Search("BC", 25).Returns(expected);

      // Act
      var actual = _controller.GetVouchersByNameSearch("BC");

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetCheapestVoucherByProductCode_StateUnderTest_ExpectedBehavior()
    {
      // Arrange
      var expected = _fixture.Create<VoucherModel>();
      _repository.GetCheapestVoucherByProductCode("BC").Returns(expected);

      // Act
      var actual = _controller.GetCheapestVoucherByProductCode("BC");

      // Assert
      Assert.Equal(expected, actual);
    }
  }
}
