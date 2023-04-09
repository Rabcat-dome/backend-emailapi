using Microsoft.Extensions.Logging;

namespace PTTDigital.Email.Common.Helper.LogExtension;

internal class CustomLogger : ILogger
{
    private readonly ILogger logger;

    internal IApplicationUser ApplicationUser { get; }

    internal CustomLogger(ILogger logger, IApplicationUser applicationUser)
    {
        ApplicationUser = applicationUser;
        this.logger = logger;
    }

    IDisposable ILogger.BeginScope<TState>(TState state)
    {
        return logger.BeginScope(state);
    }

    bool ILogger.IsEnabled(LogLevel logLevel)
    {
        return logger.IsEnabled(logLevel);
    }

    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, System.Exception exception, Func<TState, System.Exception, string> formatter)
    {
        logger.Log(logLevel, eventId, state, exception, formatter);
    }
}
