using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Services.Voucher.Contracts;
using Services.Voucher.Features.AspNet.Authorization;
using Services.Voucher.Features.AspNet.Authorization.Swagger;
using Services.Voucher.Features.AspNet.HealthChecks;
using Services.Voucher.Features.AspNet.Versioning.Swagger;
using Services.Voucher.Features.Vouchers;

namespace Services.Voucher
{
  [ExcludeFromCodeCoverage]
  public class Startup
  {
    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
      Configuration = configuration;
      Env = env;
    }

    private IConfiguration Configuration { get; }
    private IWebHostEnvironment Env { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      const string serviceName = "Voucher API";

      services.AddLogging();
      services.AddOpenTelemetryTracing(builder =>
      {
        builder
          .AddSource(serviceName)
          .AddHttpClientInstrumentation()
          .AddAspNetCoreInstrumentation()
          .SetResourceBuilder(
            ResourceBuilder.CreateDefault()
              .AddEnvironmentVariableDetector()
              .AddService(serviceName: serviceName,
                serviceVersion: typeof(Startup).Assembly.GetName().Version?.ToString()));
        if (Env.IsDevelopment() && Debugger.IsAttached)
          builder.AddConsoleExporter();
      });

      services.AddCors(c =>
        c.AddDefaultPolicy(p => p.AllowAnyOrigin().WithMethods("GET").DisallowCredentials()));

      if (Env.IsDevelopment())
      {
        services.AddInMemoryApiKeyRolesProvider(new Dictionary<string, string[]>
        {
          ["034ede0baffa42e4a1186f88ff2b825b"] = new[] { "VOUCHER:READ" }
        });
      }
      services.AddAuthentication(c =>
      {
        c.DefaultScheme = ApiKeys.SchemeName;
        c.AddScheme<ApiKeyAuthenticationHandler>(ApiKeys.SchemeName, ApiKeys.SchemeName);
      });
      services.AddAuthorization(c => c.AddPolicy("VOUCHER:READ", p => p.RequireRole("VOUCHER:READ")));

      services.AddHealthChecks().AddStartupCheck();

      services.AddSingleton<IVoucherProvider>(new JsonFileVoucherProvider());
      services.AddInMemoryVouchers(c => c.SearchProvider = SearchProviders.Lucene);

      services.AddApiVersioning(options =>
      {
        options.ReportApiVersions = true;
        options.DefaultApiVersion = ApiVersion.Default;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
      });
      services.AddControllers();
      services.AddSwaggerGen(c =>
      {
        c.DocumentFilter<VersioningOperationFilter>();
        c.OperationFilter<ApiKeyOperationFilter>();
        var securityScheme = new OpenApiSecurityScheme
        {
          Type = SecuritySchemeType.ApiKey,
          Scheme = ApiKeys.SchemeName,
          In = ParameterLocation.Header,
          Name = ApiKeys.HeaderName,
          Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = ApiKeys.SchemeName },
        };
        c.AddSecurityDefinition(ApiKeys.SchemeName, securityScheme);
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
          [securityScheme] = new List<string>()
        });
        c.SwaggerDoc("1.0", new OpenApiInfo { Title = serviceName, Version = "1.0" });
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app)
    {
      // app.UseHttpsRedirection();
      app.UseRouting();

      if (Env.IsDevelopment()) app.UseDeveloperExceptionPage();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseSwaggerUI(c =>
      {
        c.EnablePersistAuthorization();
        c.EnableTryItOutByDefault();
        c.SwaggerEndpoint("1.0/swagger.json", "Voucher API 1.0");
      });

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapHealthChecks("/healthz/ready", new HealthCheckOptions
        {
          Predicate = healthCheck => healthCheck.Tags.Contains("ready")
        });
        endpoints.MapHealthChecks("/healthz/live", new HealthCheckOptions
        {
          Predicate = _ => false
        });
        endpoints.MapSwagger();
        endpoints.MapControllers();
      });
    }
  }
}
