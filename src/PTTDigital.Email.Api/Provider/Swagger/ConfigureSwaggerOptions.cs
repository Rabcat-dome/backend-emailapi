using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PTTDigital.Email.Api.Provider.Swagger;

/// <summary>
/// Creating .NET Core API and Swagger UI with versioning
/// <list type="table">See: <see href="https://medium.com/@hendisuhardja/creating-net-core-api-and-swagger-ui-with-versioning-e21979a54d2c"/></list>
/// </summary>
internal class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider provider;
    private readonly IOpenApiOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
    /// </summary>
    /// <param name="provider">The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger documents.</param>
    /// <param name="options"></param>
    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IOpenApiOptions options)
    {
        this.provider = provider;
        this.options = options;
    }

    /// <inheritdoc />
    public void Configure(SwaggerGenOptions options)
    {
        if (provider == null)
        {
            return;
        }

        // add a swagger document for each discovered API version
        // note: you might choose to skip or document deprecated API versions differently
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }
    }

    private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var info = new OpenApiInfo()
        {
            Title = options?.Title,
            Version = $"v{description.ApiVersion?.ToString() ?? "1.0"}",
            Contact = new OpenApiContact
            {
                Name = options?.ContactName,
                Email = options?.ContactEmail,
            },
            License = new OpenApiLicense
            {
                Name = options?.LicenseName,
            },
        };

        if (description.IsDeprecated)
        {
            info.Description += " This API version has been deprecated.";
        }
        return info;
    }
}
