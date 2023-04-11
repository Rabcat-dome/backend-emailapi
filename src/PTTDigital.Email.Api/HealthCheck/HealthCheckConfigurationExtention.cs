using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PTTDigital.Email.Common.Configuration.AppSetting;

namespace PTTDigital.Email.Api.HealthCheck;

internal static class HealthCheckConfigurationExtention
{
    private const string pathReady = "ready";
    private const string pathAlive = "alive";
    private static string[] tags = new[] { pathReady, pathAlive };

    internal static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddSingleton<IHealthCheckPath>(new HealthCheckPath(tags));

        services.AddSingleton<StartupHealthCheck>();
        services.AddHostedService<StartupBackgroundService>();

        services.AddHealthChecks()
            .AddCheck<StartupHealthCheck>("Startup", tags: new[] { pathReady })
            .AddCheck<ContextHealthCheck>("Database", tags: new[] { pathAlive })
            .AddCheck<AzureADHealthCheck>("AzureAD", tags: new[] { pathReady })
            .AddCheck<AzureDBHealthCheck>("AzureDB", tags: new[] { pathReady })
            .AddCheck<KeyVaultHealthCheck>("KeyVault", tags: new[] { pathReady });


        return services;
    }

    internal static IEndpointRouteBuilder UseCustomHealthCheck(this WebApplication app)
    {
        var appName = app.Services.GetService<IAppSetting>()?.AppName;
        app.MapHealthChecks($"/{pathReady}", new HealthCheckOptions
        {
            Predicate = healthCheck => {
                var result = healthCheck.Tags.All(c => tags.Contains(c));
                return result;
            },
            ResponseWriter = async (context, report) =>
            {
                await CreateResponseWriter(context, report, appName, true);
            }
        });

        app.MapHealthChecks($"/{pathAlive}", new HealthCheckOptions
        {
            Predicate = healthCheck => {
                var result = healthCheck.Tags.Any(c => tags.Contains(c));
                return result;
            },
            ResponseWriter = async (context, report) =>
            {
                await CreateResponseWriter(context, report, appName, false);
            }
        });

        return app;
    }

    private static async Task CreateResponseWriter(HttpContext context, HealthReport report, string? appName, bool isHealthyAll)
    {
        var detail = report.Entries.Select(c => new
        {
            Name = c.Key,
            Status = c.Value.Status.ToString(),
            //c.Value.Description,
        }).ToArray();

        var fileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
        var isHealthy = isHealthyAll ? report.Entries.All(c => c.Value.Status == HealthStatus.Healthy)
                                     : report.Entries.Any(c => c.Value.Status == HealthStatus.Healthy);
        var result = new
        {
            IsHealthy = isHealthy,
            AppName = appName,
            Version = fileVersionInfo.ProductVersion,
            Detail = detail
        };

        context.Response.StatusCode = isHealthy ? StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable;

        var options = new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web)
        {
            WriteIndented = true,
        };

        await context.Response.WriteAsJsonAsync(result, options);
    }
}
