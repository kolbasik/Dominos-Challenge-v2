using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;
using AutoFixture;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Services.Voucher.Authorization;
using Services.Voucher.Models;
using Xunit;

namespace Services.Voucher.Test.Unit
{
  public class ServerTests
  {
    private readonly Fixture _fixture;
    private readonly TestServer _testServer;

    public ServerTests()
    {
      _fixture = new Fixture();
      _testServer = new TestServer(
        new WebHostBuilder()
          .UseEnvironment(Environments.Development)
          .UseStartup<Startup>());
    }

    [Fact]
    public async Task VouchersEndpoint_ShouldReturnsVouchers_WhenAuthorized()
    {
      // Arrange
      var httpClient = _testServer.CreateClient();
      httpClient.DefaultRequestHeaders.Add(ApiKeys.HeaderName, "034ede0baffa42e4a1186f88ff2b825b");

      // Act
      var vouchers = await httpClient.GetFromJsonAsync<VoucherModel[]>("/api/1.0/Voucher/Get");
      Assert.Equal(25, vouchers.Length);

      var expectedVoucher = vouchers.First();
      var voucherById = await httpClient.GetFromJsonAsync<VoucherModel>($"/api/1.0/Voucher/GetVoucherById?id={expectedVoucher.Id}");
      Assert.Equal(expectedVoucher, voucherById);

      var vouchersByName = await httpClient.GetFromJsonAsync<VoucherModel[]>($"/api/1.0/Voucher/GetVouchersByName?name={HttpUtility.UrlEncode(expectedVoucher.Name)}");
      Assert.All(vouchersByName, it => Assert.Equal(expectedVoucher.Name, it.Name));

      var expectedProductCode = expectedVoucher.ProductCodes.Split(",").First();
      var cheapestVoucher = await httpClient.GetFromJsonAsync<VoucherModel>($"/api/1.0/Voucher/GetCheapestVoucherByProductCode?productCode={HttpUtility.UrlEncode(expectedProductCode)}");
      Assert.Contains(expectedProductCode, cheapestVoucher.ProductCodes);

      var expectedWords = new [] { "Chicken", "Garlic", "Pizzas" };
      var vouchersBySearch = await httpClient.GetFromJsonAsync<VoucherModel[]>($"/api/1.0/Voucher/GetVouchersByNameSearch?search={HttpUtility.UrlEncode(string.Join(" ", expectedWords))}");
      Assert.All(vouchersBySearch, it => Assert.All(expectedWords, word => Assert.Contains(word, it.Name)));
    }


    [Fact]
    public async Task VouchersEndpoint_ShouldReturns401_WhenAnonymous()
    {
      // Arrange
      var httpClient = _testServer.CreateClient();

      // Act
      var responseGet = await httpClient.GetAsync("/api/1.0/Voucher/Get");
      Assert.Equal(HttpStatusCode.Unauthorized, responseGet.StatusCode);

      var responseGetById = await httpClient.GetAsync("/api/1.0/Voucher/GetVoucherById?id=123");
      Assert.Equal(HttpStatusCode.Unauthorized, responseGetById.StatusCode);

      var responseGetByName = await httpClient.GetAsync("/api/1.0/Voucher/GetVouchersByName?name=123");
      Assert.Equal(HttpStatusCode.Unauthorized, responseGetByName.StatusCode);

      var responseCheapestByProductCode = await httpClient.GetAsync("/api/1.0/Voucher/GetCheapestVoucherByProductCode?productCode=123");
      Assert.Equal(HttpStatusCode.Unauthorized, responseCheapestByProductCode.StatusCode);

      var responseGetBySearch = await httpClient.GetAsync("/api/1.0/Voucher/GetVouchersByNameSearch?search=123");
      Assert.Equal(HttpStatusCode.Unauthorized, responseGetBySearch.StatusCode);
    }
  }
}
