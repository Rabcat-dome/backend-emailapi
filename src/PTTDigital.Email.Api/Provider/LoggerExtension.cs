using Microsoft.Extensions.DependencyInjection.Extensions;
using PTTDigital.Email.Common.Helper.LogExtension;
using Serilog;

namespace PTTDigital.Email.Api.Provider;

internal static partial class LoggerExtension
{
    internal static IHostBuilder AddLogger(this WebApplicationBuilder builder)
    {
        builder.Services.RemoveAll<ILoggerHelper>().AddSingleton<ILoggerHelper, LoggerHelper>();

        return builder.Host.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddSerilog();
        }).UseSerilog((context, config) =>
        {
            config.ReadFrom.Configuration(context.Configuration);
        });
    }
}