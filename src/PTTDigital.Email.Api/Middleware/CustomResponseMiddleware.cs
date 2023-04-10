using PTTDigital.Email.Api.Helper;
using PTTDigital.Email.Common;
using PTTDigital.Email.Common.ApplicationUser.User;
using PTTDigital.Email.Common.Helper.LogExtension;

namespace PTTDigital.Email.Api.Middleware;

internal class CustomResponseMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomResponseMiddleware> _logger;

    internal record ResponseInfo(int StatusCode, string Message);

    public CustomResponseMiddleware(RequestDelegate next, ILogger<CustomResponseMiddleware> logger)
    {
        this._next = next;
        this._logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IApplicationUser appUser)
    {
        var originalBody = context.Response.Body;

        try
        {
            context.Response.OnStarting(async obj =>
            {
                if (obj is HttpContext ctx)
                {
                    ctx.Response.Headers["X-TraceId"] = ctx.TraceIdentifier;
                }

                await Task.CompletedTask;
            }, context);

            using var memStream = new MemoryStream();
            context.Response.Body = memStream;

            await _next(context);

            memStream.Seek(0, SeekOrigin.Begin);
            await memStream.CopyToAsync(originalBody);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Warning, LogEvents.Middleware, "Unhandled Exception occurred", appUser, ex);

            var response = CreateCustomResponse(context, ex);

            context.Response.Body = originalBody;
            context.Response.StatusCode = response.StatusCode;
            await context.Response.WriteAsJsonAsync(ResultMessage.Error(response?.Message!));
        }
    }

    private static ResponseInfo CreateCustomResponse(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            InvalidOperationException => exception.Source?.Equals("Microsoft.AspNetCore.Authorization") ?? false
                ? StatusCodes.Status401Unauthorized
                : StatusCodes.Status503ServiceUnavailable,

              Exception => StatusCodes.Status500InternalServerError,
            _ => context.Response.StatusCode
        };

        var message = statusCode switch
        {
            StatusCodes.Status404NotFound => $"Invalid request Path: '{context.Request.Path}'",
            _ => exception?.FromHierarchy(c => c.InnerException!)?.LastOrDefault()?.Message
        };

        return new ResponseInfo(statusCode, message?.Trim()!);
    }
}