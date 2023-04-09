using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PTTDigital.Email.Api.Provider.Swagger;

internal class CustomDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var paths = new OpenApiPaths();
        swaggerDoc.Paths.OrderBy(e => e.Key).Select(c => new { c.Key, c.Value }).ToList().ForEach(item =>
        {
            paths.Add(item.Key, item.Value);
        });
        swaggerDoc.Paths = paths;
    }
}
