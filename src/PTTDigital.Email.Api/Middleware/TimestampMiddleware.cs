using System.Diagnostics;

namespace PTTDigital.Email.Api.Middleware;

internal class TimestampMiddleware
{
    private readonly RequestDelegate _next = null;

    public TimestampMiddleware(RequestDelegate next)
    {
        this._next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        context.Response.OnStarting(async obj =>
        {
            if (obj is HttpContext ctx)
            {
                ctx.Response.Headers["X-ResponseTime"] = stopwatch.Elapsed.ToString();
            }

            await Task.CompletedTask;
        }, context);

        await _next(context);
    }
}