using PTTDigital.Email.Application.Api.Client.HttpDelegating;
using PTTDigital.Email.Common.ApplicationUser.User;
using PTTDigital.Email.Common.Configuration.AppSetting;

namespace PTTDigital.Email.Api.Provider;

internal static partial class HttpClientFactoryExtension
{
    /// <summary>
    /// Adds a singleton service of HttpClients and ApiClientFactory with a
    /// factory specified in <see cref="ApiClientType"/> to the
    /// specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="clientType">The type of the ApiClient to add.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    /// <seealso cref="ServiceLifetime.Singleton"/>

    internal static IServiceCollection AddHttpClientFactory(this IServiceCollection services,
        ApiClientType clientType = ApiClientType.All)
    {
        services.AddScoped<IApplicationUser, ApplicationUser>();
        services.AddScoped<IApiConfigurationFactory, ApiConfigurationFactory>();

        var apiClientTypes = GetFlags(clientType).Except(new Enum[] { ApiClientType.None, ApiClientType.All })
            .OfType<ApiClientType>().ToList();

        apiClientTypes.ForEach(apiClientType =>
        {
            var name = apiClientType.ToString();
            services.AddHttpClient(name, (service, httpClient) =>
            {
                using var scope = service.CreateScope();
                var configFactory = scope.ServiceProvider.GetService<IApiConfigurationFactory>();
                var apiConfig = configFactory.GetConfiguration(apiClientType);
                if (string.IsNullOrWhiteSpace(apiConfig?.BaseUri))
                {
                    return;
                }

                httpClient.BaseAddress = new Uri(apiConfig.BaseUri);
            }).AddHttpMessageHandler<OperationHandler>(); ;
                //.AddHttpMessageHandler<OperationHandler>();
            //.AddHttpMessageHandler<LoggingDelegatingHandler>();
        });

        services.AddScoped<OperationHandler>();
        //services.AddScoped<OperationHandler>();
        //services.AddScoped<LoggingDelegatingHandler>();

        return services;
    }

    private static IEnumerable<Enum> GetFlags(Enum input)
    {
        foreach (Enum value in Enum.GetValues(input.GetType()))
            if (input.HasFlag(value))
                yield return value;
    }
}