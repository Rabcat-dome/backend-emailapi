namespace PTTDigital.Email.Api.Provider.Swagger;

internal static class SwaggerGenServiceProviderExtension
{
    internal static IServiceCollection AddSwaggerGenVersioning(this IServiceCollection services)
    {
        return services.AddSwaggerGenVersioning(options =>
        {
            options.Title = AppDomain.CurrentDomain.FriendlyName;
            options.ContactName = "Technical Support";
            options.ContactEmail = "support@pttdigital.com";
            options.LicenseName = "PTT Digital Solutions Company Limited";
        });
    }

    internal static IServiceCollection AddSwaggerGenVersioning(this IServiceCollection services, Action<IOpenApiOptions> configureOptions)
    {
        IOpenApiOptions openApiOptions = new OpenApiOptions();
        configureOptions(openApiOptions);
        services.TryAddSingleton(openApiOptions);

        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
        });

        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>(serviceProvider =>
        {
            var options = serviceProvider.GetService<IOpenApiOptions>();
            var provider = serviceProvider.GetService<IApiVersionDescriptionProvider>();
            return new ConfigureSwaggerOptions(provider, options);
        });

        services.AddSwaggerGen(options =>
        {
            var authName = JwtBearerDefaults.AuthenticationScheme;
            // add a custom operation filter which sets default values
            options.OperationFilter<SwaggerDefaultValues>();
            options.DocumentFilter<CustomDocumentFilter>();

            options.IgnoreObsoleteActions();
            options.IgnoreObsoleteProperties();
            options.AddSecurityDefinition(authName, new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = $"Please insert JWT Authorization with {authName} into field",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme{
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = authName
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Integrate xml comments
            foreach (var file in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml"))
            {
                options.IncludeXmlComments(file);
            }
        });

        return services;
    }
}