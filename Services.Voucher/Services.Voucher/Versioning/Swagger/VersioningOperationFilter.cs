using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Services.Voucher.Versioning.Swagger
{
  [ExcludeFromCodeCoverage]
  public class VersioningOperationFilter : IDocumentFilter
  {
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
      var paths = swaggerDoc.Paths;
      swaggerDoc.Paths = new OpenApiPaths();
      foreach (var path in paths)
      {
        swaggerDoc.Paths.Add(path.Key.Replace("{version}", context.DocumentName), path.Value);
        foreach (var operation in path.Value.Operations)
        {
          var version = operation.Value.Parameters.FirstOrDefault(p => string.Equals(p.Name, "version", StringComparison.Ordinal));
          if (version != null)
          {
            operation.Value.Parameters.Remove(version);
          }
        }
      }
    }
  }
}
