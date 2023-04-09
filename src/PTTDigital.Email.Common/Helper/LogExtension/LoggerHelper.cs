using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PTTDigital.Email.Common.Helper.LogExtension;

public class LoggerHelper : ILoggerHelper
{
    private readonly IServiceProvider _serviceProvider;

    public LoggerHelper(IServiceProvider serviceProvider)
    {
        this._serviceProvider = serviceProvider;
    }

    ILogger ILoggerHelper.CreateLogger(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var categoryName = TypeExtensions.GetTypeDisplayName(type);
        var factory = _serviceProvider.GetService<ILoggerFactory>();
        var logger = factory.CreateLogger(categoryName);
        var appUser = new ApplicationUser.User.ApplicationUser(_serviceProvider);

        return new CustomLogger(logger, appUser);
    }
}
