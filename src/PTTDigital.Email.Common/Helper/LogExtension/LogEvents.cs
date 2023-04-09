using Microsoft.Extensions.Logging;

namespace PTTDigital.Email.Common.Helper.LogExtension;

public static class LogEvents
{
    private static readonly EventId middleware = new(9000, nameof(Middleware));
    private static readonly EventId controller = new(9100, nameof(Controller));
    private static readonly EventId internalService = new(9200, nameof(InternalService));

    public static EventId Middleware => middleware;
    public static EventId Controller => controller;
    public static EventId InternalService => internalService;
}
