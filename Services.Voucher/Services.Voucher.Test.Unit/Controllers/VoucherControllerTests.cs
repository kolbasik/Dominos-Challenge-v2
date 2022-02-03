﻿using NSubstitute;
using Services.Voucher.Controllers;
using Services.Voucher.Models;
using Services.Voucher.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Services.Voucher.Test.Unit.Controllers
{
    public class VoucherControllerTests
    {
        private readonly VoucherController _controller;
        private readonly VoucherRepository _repository;

        public VoucherControllerTests()
        {
            _controller = new VoucherController();
            _repository = Substitute.For<VoucherRepository>();
            _controller.Repository = _repository;
        }

        [Fact]
        public void Get_ShouldReturnAllVouchers_WhenNoCountIsProvided()
        {
            // Arrange
            var vouchers = new List<VoucherModel>();
            for (var i = 0; i < 1000; i++)
            {
                vouchers.Add(new VoucherModel
                {
                    Id = new Guid()
                });
            }
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
            int count = 5;
            var vouchers = new List<VoucherModel>();
            for (var i = 0; i < 1000; i++)
            {
                vouchers.Add(new VoucherModel
                {
                    Id = new Guid()
                });
            }
            _repository.GetVouchers().Returns(vouchers);

            // Act
            var result = _controller.Get(count);

            // Assert
            Assert.Equal(count, result.Count());
        }

        [Fact]
        public void GetVoucherById_StateUnderTest_ExpectedBehavior()
        {
            // TODO
        }

        [Fact]
        public void GetVouchersByName_ShouldReturnAllVouchersWithTheGivenSearchString_WhenVoucherExists()
        {
            // Arrange
            var vouchers = new List<VoucherModel>();
            vouchers.Add(new VoucherModel { Id = new Guid(), Name = "A" });
            vouchers.Add(new VoucherModel { Id = new Guid(), Name = "A" });
            vouchers.Add(new VoucherModel { Id = new Guid(), Name = "B" });
            _repository.GetVouchers().Returns(vouchers);

            // Act
            var result = _controller.GetVouchersByName("A");

            // Assert
            Assert.Equal(result, new List<VoucherModel>() { vouchers.ElementAt(0), vouchers.ElementAt(1) });
        }

        [Fact]
        public void GetVouchersByNameSearch_ShouldReturnAllVouchersThatContainTheGivenSearchString_WhenVoucherExists()
        {
            // Arrange
            var vouchers = new List<VoucherModel>();
            vouchers.Add(new VoucherModel { Id = new Guid(), Name = "ABC"  });
            vouchers.Add(new VoucherModel { Id = new Guid(), Name = "ABCD" });
            vouchers.Add(new VoucherModel { Id = new Guid(), Name = "ACD" });
            _repository.GetVouchers().Returns(vouchers);

            // Act
            var result = _controller.GetVouchersByNameSearch("BC");

            // Assert
            Assert.Equal(result, new List<VoucherModel>() { vouchers.ElementAt(0), vouchers.ElementAt(1) });
        }

        [Fact]
        public void GetCheapestVoucherByProductCode_StateUnderTest_ExpectedBehavior()
        {
            // TODO
        }

        // TODO: This is not all the tests that we would like to see + the above tests can be made much smarter.
    }
}
