namespace PTTDigital.Email.Common.Helper.LogExtension;

public class LoggerHelper : ILoggerHelper
{
    private readonly IServiceProvider serviceProvider;

    public LoggerHelper(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    ILogger ILoggerHelper.CreateLogger(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var categoryName = Microsoft.TypeExtensions.GetTypeDisplayName(type);
        var factory = serviceProvider.GetService<ILoggerFactory>();
        var logger = factory.CreateLogger(categoryName);
        var appUser = new ApplicationUser.User.ApplicationUser(serviceProvider);

        return new CustomLogger(logger, appUser);
    }
}
