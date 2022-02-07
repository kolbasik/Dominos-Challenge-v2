using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Services.Voucher.Features.AspNet.Authorization.Swagger
{
  [ExcludeFromCodeCoverage]
  public class ApiKeyOperationFilter : IOperationFilter
  {
    private readonly OpenApiSecurityScheme _securityScheme = new ()
    {
      Type = SecuritySchemeType.OAuth2,
      Scheme = ApiKeys.SchemeName,
      In = ParameterLocation.Header,
      Name = ApiKeys.HeaderName,
      Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = ApiKeys.SchemeName }
    };

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
      var endpointMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
      var authorize = endpointMetadata.FirstOrDefault(filter => filter is AuthorizeAttribute) as AuthorizeAttribute;
      var allowAnonymous = endpointMetadata.Any(filter => filter is AllowAnonymousAttribute);
      if (authorize != null && !allowAnonymous)
      {
        var requirement = new OpenApiSecurityRequirement { [_securityScheme] = new List<string>() };
        operation.Security ??= new List<OpenApiSecurityRequirement>();
        operation.Security.Add(requirement);
      }
    }
  }
}
