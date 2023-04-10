using PTTDigital.Email.Api.HealthCheck;
using PTTDigital.Email.Common.Configuration.AppSetting;
using System.Net;

namespace PTTDigital.Email.Api.Middleware;

public class ApiRouterMiddleware
{
    private readonly RequestDelegate next;
    private readonly IHostEnvironment environment;
    private readonly IHealthCheckPath healthCheckPath;

    public ApiRouterMiddleware(RequestDelegate next,
                               IHostEnvironment environment,
                               IHealthCheckPath healthCheckPath)
    {
        this.next = next;
        this.environment = environment;
        this.healthCheckPath = healthCheckPath;
    }

    public async Task InvokeAsync(HttpContext context,
                                  IAppSetting appSetting)
    {
        var apiKey = context.GetHeaderKey("X-ApiKey");
        var apiKeyFromConfig = appSetting.ApiClients![ApiClientType.ApiAuth.ToString()]!.ApiKey;
        var path = context.Request.Path.ToString().ToLowerInvariant();

        if (healthCheckPath.IsByPass(path))
        {
            await next(context);
            return;
        }

        if (!apiKey!.Contains(apiKeyFromConfig) && !environment.IsDevelopment())
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync($"From project auth ApiKey = {apiKey} is invalid.");
            return;
        }

        if (!path.StartsWith("/api"))
        {
            await next(context);
            return;
        }

        await next(context);
    }
}