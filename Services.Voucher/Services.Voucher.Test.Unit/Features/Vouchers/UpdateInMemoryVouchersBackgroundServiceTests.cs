using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Services.Voucher.Contracts;
using Services.Voucher.Contracts.Models;
using Services.Voucher.Features.ServiceAccessors;
using Services.Voucher.Features.Vouchers;
using Xunit;

namespace Services.Voucher.Test.Unit.Features.Vouchers
{
  public class UpdateInMemoryVouchersBackgroundServiceTests
  {
    [Fact]
    public void Constructor_ShouldCreateABackgroundService()
    {
      // Arrange
      var options = Substitute.For<IOptions<UpdateInMemoryVouchersBackgroundOptions>>();
      options.Value.Returns(new UpdateInMemoryVouchersBackgroundOptions());
      var logger = Substitute.For<ILogger<UpdateInMemoryVouchersBackgroundService>>();
      var healthCheck = new InMemoryVouchersHealthCheck();
      var voucherProvider = Substitute.For<IVoucherProvider>();
      var voucherRepository = new ServiceAccessor<IVoucherRepository>();
      var voucherSearch = new ServiceAccessor<IVoucherSearch>();

      // Act
      var service = new UpdateInMemoryVouchersBackgroundService(options, logger, healthCheck, voucherProvider, voucherRepository, voucherSearch);

      // Assert
      Assert.NotNull(service);
    }

    [Fact]
    public async Task StartAsync_ShouldPopulateVouchersInMemory_IfLuceneIsChosen()
    {
      // Arrange
      var options = Substitute.For<IOptions<UpdateInMemoryVouchersBackgroundOptions>>();
      options.Value.Returns(new UpdateInMemoryVouchersBackgroundOptions
      {
        SearchProvider = SearchProviders.Lucene,
        UpdateInterval = TimeSpan.FromMilliseconds(5)
      });
      var logger = Substitute.For<ILogger<UpdateInMemoryVouchersBackgroundService>>();
      var healthCheck = new InMemoryVouchersHealthCheck();
      var voucherProvider = Substitute.For<IVoucherProvider>();
      var voucherRepository = new ServiceAccessor<IVoucherRepository>();
      var voucherSearch = new ServiceAccessor<IVoucherSearch>();

      var service = new UpdateInMemoryVouchersBackgroundService(options, logger, healthCheck, voucherProvider, voucherRepository, voucherSearch);
      using var cts = new CancellationTokenSource();
      cts.CancelAfter(TimeSpan.FromMilliseconds(200));

      // Act
      await Task.WhenAll(service.StartAsync(cts.Token), Task.Delay(TimeSpan.FromMilliseconds(50), cts.Token));

      // Assert
      Assert.NotNull(service);
      Assert.Equal(2, (int) healthCheck.Status);
      Assert.InRange(voucherProvider.GetVouchers().ReceivedCalls().Count(), 2, int.MaxValue);
      Assert.NotNull(voucherRepository.Current);
      Assert.NotNull(voucherSearch.Current);
      Assert.IsAssignableFrom<InMemoryLuceneVoucherSearch>(voucherSearch.Current);
    }

    [Fact]
    public async Task StartAsync_ShouldPopulateVouchersInMemory_IfTrigramIsChosen()
    {
      // Arrange
      var options = Substitute.For<IOptions<UpdateInMemoryVouchersBackgroundOptions>>();
      options.Value.Returns(new UpdateInMemoryVouchersBackgroundOptions
      {
        SearchProvider = SearchProviders.Trigrams,
        UpdateInterval = TimeSpan.FromMilliseconds(5)
      });
      var logger = Substitute.For<ILogger<UpdateInMemoryVouchersBackgroundService>>();
      var healthCheck = new InMemoryVouchersHealthCheck();
      var voucherProvider = Substitute.For<IVoucherProvider>();
      var voucherRepository = new ServiceAccessor<IVoucherRepository>();
      var voucherSearch = new ServiceAccessor<IVoucherSearch>();

      var service = new UpdateInMemoryVouchersBackgroundService(options, logger, healthCheck, voucherProvider, voucherRepository, voucherSearch);
      using var cts = new CancellationTokenSource();
      cts.CancelAfter(TimeSpan.FromMilliseconds(200));

      // Act
      await Task.WhenAll(service.StartAsync(cts.Token), Task.Delay(TimeSpan.FromMilliseconds(50), cts.Token));

      // Assert
      Assert.NotNull(service);
      Assert.Equal(2, (int) healthCheck.Status);
      Assert.InRange(voucherProvider.GetVouchers().ReceivedCalls().Count(), 2, int.MaxValue);
      Assert.NotNull(voucherRepository.Current);
      Assert.NotNull(voucherSearch.Current);
      Assert.IsAssignableFrom<InMemoryTrigramVoucherSearch>(voucherSearch.Current);
    }

    [Fact]
    public async Task StartAsync_ShouldIndicateUnhealthy_IfUnableToPopulateVouchers()
    {
      // Arrange
      var options = Substitute.For<IOptions<UpdateInMemoryVouchersBackgroundOptions>>();
      options.Value.Returns(new UpdateInMemoryVouchersBackgroundOptions
      {
        RetryOnFailure = TimeSpan.FromMilliseconds(5)
      });
      var logger = Substitute.For<ILogger<UpdateInMemoryVouchersBackgroundService>>();
      var healthCheck = new InMemoryVouchersHealthCheck();
      var voucherProvider = Substitute.For<IVoucherProvider>();
      voucherProvider.GetVouchers().Throws(new Exception("NETWORK_ISSUE"));
      var voucherRepository = new ServiceAccessor<IVoucherRepository>();
      var voucherSearch = new ServiceAccessor<IVoucherSearch>();

      var service = new UpdateInMemoryVouchersBackgroundService(options, logger, healthCheck, voucherProvider, voucherRepository, voucherSearch);
      using var cts = new CancellationTokenSource();
      cts.CancelAfter(TimeSpan.FromMilliseconds(50));

      // Act
      await service.StartAsync(cts.Token);

      // Assert
      Assert.NotNull(service);
      Assert.Null(voucherRepository.Current);
      Assert.Null(voucherSearch.Current);
      Assert.Equal(0, (int) healthCheck.Status);
    }

    [Fact]
    public async Task StartAsync_ShouldIndicateDegraded_IfUnableToUpdateVouchers()
    {
      // Arrange
      var options = Substitute.For<IOptions<UpdateInMemoryVouchersBackgroundOptions>>();
      options.Value.Returns(new UpdateInMemoryVouchersBackgroundOptions
      {
        UpdateInterval = TimeSpan.Zero,
        RetryOnFailure = TimeSpan.FromMilliseconds(5)
      });
      var logger = Substitute.For<ILogger<UpdateInMemoryVouchersBackgroundService>>();
      var healthCheck = new InMemoryVouchersHealthCheck();
      var voucherProvider = Substitute.For<IVoucherProvider>();
      voucherProvider.GetVouchers().Returns(
        _ => new List<VoucherModel>(),
        _ => throw new Exception("NETWORK_ISSUE"));
      var voucherRepository = new ServiceAccessor<IVoucherRepository>();
      var voucherSearch = new ServiceAccessor<IVoucherSearch>();

      var service = new UpdateInMemoryVouchersBackgroundService(options, logger, healthCheck, voucherProvider, voucherRepository, voucherSearch);
      using var cts = new CancellationTokenSource();
      cts.CancelAfter(TimeSpan.FromMilliseconds(200));

      // Act
      await Task.WhenAll(service.StartAsync(cts.Token), Task.Delay(TimeSpan.FromMilliseconds(50), cts.Token));

      // Assert
      Assert.NotNull(service);
      Assert.NotNull(voucherRepository.Current);
      Assert.NotNull(voucherSearch.Current);
      Assert.Equal(1, (int) healthCheck.Status);
    }

    [Fact]
    public async Task StopAsync_ShouldStopTheService()
    {
      // Arrange
      var options = Substitute.For<IOptions<UpdateInMemoryVouchersBackgroundOptions>>();
      options.Value.Returns(new UpdateInMemoryVouchersBackgroundOptions
      {
        UpdateInterval = TimeSpan.FromMilliseconds(5)
      });
      var logger = Substitute.For<ILogger<UpdateInMemoryVouchersBackgroundService>>();
      var healthCheck = new InMemoryVouchersHealthCheck();
      var voucherProvider = Substitute.For<IVoucherProvider>();
      var voucherRepository = new ServiceAccessor<IVoucherRepository>();
      var voucherSearch = new ServiceAccessor<IVoucherSearch>();

      var service = new UpdateInMemoryVouchersBackgroundService(options, logger, healthCheck, voucherProvider, voucherRepository, voucherSearch);
      using var cts = new CancellationTokenSource();
      cts.CancelAfter(TimeSpan.FromMilliseconds(200));

      // Act
      await service.StartAsync(cts.Token);
      await service.StopAsync(cts.Token);

      // Assert
      Assert.NotNull(service);
      Assert.Equal(2, (int) healthCheck.Status);
    }
  }
}
