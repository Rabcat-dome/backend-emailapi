namespace PTTDigital.Email.Api.Provider.Swagger;

internal static class SwaggerGenApplicationBuilderExtension
{
    internal static IApplicationBuilder UseSwaggerVersioning(this IApplicationBuilder app)
    {
        var webApp = app as WebApplication;
        var appName = webApp?.Environment.ApplicationName;
        var provider = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            if (provider == null)
                return;

            options.DocumentTitle = appName;
            options.DocExpansion(DocExpansion.None);

            // build a swagger endpoint for each discovered API version
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"{appName} v{description.ApiVersion}");
            }
        });
        return app;
    }
}
