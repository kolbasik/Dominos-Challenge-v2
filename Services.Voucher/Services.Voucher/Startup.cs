using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Services.Voucher
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      const string serviceName = "Voucher API";

      services.AddControllers();
      services.AddSwaggerGen(c => { c.SwaggerDoc("1.0", new OpenApiInfo { Title = serviceName, Version = "1.0" }); });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseRouting();
      if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
      // app.UseAuthorization();
      // app.UseHttpsRedirection();

      app.UseSwaggerUI(c =>
      {
        c.EnablePersistAuthorization();
        c.EnableTryItOutByDefault();
        c.SwaggerEndpoint("1.0/swagger.json", "Voucher API 1.0");
      });

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapSwagger();
        endpoints.MapControllers();
      });
    }
  }
}
