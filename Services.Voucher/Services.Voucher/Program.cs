using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Services.Voucher
{
  [ExcludeFromCodeCoverage]
  public static class Program
  {
    public static void Main(string[] args)
    {
      CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
      return Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
          // Heroku requires to use PORT
          var port = Environment.GetEnvironmentVariable("PORT");
          if (!string.IsNullOrWhiteSpace(port))
          {
            webBuilder.UseUrls($"http://+:{port}");
          }
          webBuilder.UseStartup<Startup>();
        });
    }
  }
}
