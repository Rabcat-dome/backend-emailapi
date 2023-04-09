namespace PTTDigital.Email.Common.ApplicationUser.User;

public class ApplicationUser : IApplicationUser
{
    public string? RemoteAddress { get; }

    public string? TraceIdentifier { get; }

    public string? UserId { get; }

    public string? UserName { get; }

    public string? SessionToken { get; }

    public string? Host { get; }

    public string? UserAgent { get; }

    public string? TraceId { get; }

    public string? AppId { get; }

    public string? AppName { get; }

    public string? AccessToken { get; }

    public ApplicationUser(IServiceProvider service)
    {
        var httpContextAccessor = service?.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
        if (httpContextAccessor?.HttpContext is not null)
        {
            var httpContext = httpContextAccessor.HttpContext;
            TraceIdentifier = httpContext?.TraceIdentifier!;

            RemoteAddress = httpContext?.GetRemoteAddress()!;
            UserId = httpContext?.GetUserId();
            UserName = httpContext?.GetUserName()!;
            SessionToken = httpContext?.GetSessionToken()!;

            Host = httpContext?.GetHost()!;
            UserAgent = httpContext?.GetUserAgent()!;

            TraceId = httpContext?.GetTraceId()!;
            AppId = httpContext?.GetAppId()!;
            AppName = httpContext?.GetAppName()!;

            AccessToken = httpContext?.GetAccessToken()!;
        }
        else if (Guid.TryParse(Thread.CurrentThread.Name, out Guid taskId))
        {
            var taskStore = service?.GetService(typeof(ITaskStoreHelper)) as ITaskStoreHelper;
            var taskValue = taskStore?.GetTaskValue(taskId);
            var appUser = taskValue?.AppUser;

            TraceIdentifier = appUser?.TraceIdentifier!;

            RemoteAddress = appUser?.RemoteAddress!;
            UserId = appUser?.UserId;
            UserName = appUser?.UserName!;
            SessionToken = appUser?.SessionToken!;
            AppName = appUser?.AppName!;
        }
    }
}

internal static class HttpContextExtension
{
    private static IPAddress? localIPAddress = null!;

    //internal static string? GetAppName(this HttpContext httpContext)
    //{
    //    return httpContext?.User?.Claims?.FirstOrDefault(e => e.Type == ClaimTypes.System)?.Value;
    //}

    internal static string? GetUserId(this HttpContext httpContext)
    {
        return httpContext?.User?.Claims?.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier)?.Value;
    }

    internal static string? GetUserName(this HttpContext httpContext)
    {
        return httpContext?.User?.Claims?.FirstOrDefault(e => e.Type == ClaimTypes.Name)?.Value;
    }

    internal static string? GetSessionToken(this HttpContext httpContext)
    {
        if (httpContext is null)
        {
            return default;
        }

        if (!httpContext.Request.Headers.TryGetValue(AuthenticationConstant.HttpHeaderSessionToken, out StringValues result))
        {
            return default;
        }

        return result.ToString();
    }

    internal static string? GetWorkstationID(this HttpContext httpContext)
    {
        if (httpContext is null)
        {
            return default;
        }

        var hostname = GetHeaderValue(httpContext, AuthenticationConstant.HttpHeaderOriginHostname);

        if (!string.IsNullOrEmpty(hostname))
        {
            return hostname.ToString();
        }

        // The IP address of the remote client.
        var ipAddress = httpContext.Connection.RemoteIpAddress;

        // The current process, processes running on the local computer.
        var pid = GetHeaderValue(httpContext, AuthenticationConstant.HttpHeaderProcessIDHostname);
        var appName = httpContext.GetAppName();
        var loginName = httpContext.GetUserName();

        return $"{ipAddress}\\{appName}{(pid != null ? $"\\{pid}" : "")}\\{loginName}";
    }

    internal static string GetRemoteAddress(this HttpContext httpContext) => GetIPEndPoint(httpContext)?.ToString()!;

    internal static string GetHost(this HttpContext httpContext) => GetHeaderValue(httpContext, "Host")!;

    internal static string GetUserAgent(this HttpContext httpContext) => GetHeaderValue(httpContext, "User-Agent")!;

    internal static string GetTraceId(this HttpContext httpContext) => GetHeaderValue(httpContext, "X-TraceId")!;

    internal static string GetAppId(this HttpContext httpContext) => GetHeaderValue(httpContext, "X-AppId")!;

    internal static string GetAppName(this HttpContext httpContext) => GetHeaderValue(httpContext, "X-AppName")!;

    internal static string GetAccessToken(this HttpContext httpContext) => GetHeaderValue(httpContext, "Authorization")!;

    private static string? GetHeaderValue(HttpContext httpContext, string key)
    {
        if (httpContext is null)
        {
            return default;
        }

        if (!httpContext.Request.Headers.TryGetValue(key, out StringValues result))
        {
            return default;
        }

        return result.ToString();
    }

    private static IPEndPoint GetIPEndPoint(HttpContext httpContext)
    {
        if (httpContext is null)
        {
            return default!;
        }

        var values = GetHeaderValue(httpContext, AuthenticationConstant.HttpHeaderForwardedFor);
        if (!string.IsNullOrEmpty(values) && IPAddress.TryParse(values, out var ipAddress))
        {
            return new IPEndPoint(ipAddress, 0);
        }

        var remoteIp = httpContext.Connection?.RemoteIpAddress;
        var remotePort = httpContext.Connection?.RemotePort ?? 0;
        var remoteIpValue = remoteIp?.ToString() ?? "::1";
        var addressFamily = remoteIp?.AddressFamily ?? AddressFamily.InterNetworkV6;

        if (remoteIpValue.Equals("::1") && addressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            if (localIPAddress is null)
            {
                localIPAddress = GetLocalAddress();
            }

            return new IPEndPoint(localIPAddress, remotePort);
        }

        return new IPEndPoint(remoteIp ?? IPAddress.None, remotePort);
    }

    private static IPAddress GetLocalAddress()
    {
        var networkInterface = NetworkInterface.GetAllNetworkInterfaces()
            .Where(network =>
            {
                var isUp = network.OperationalStatus == OperationalStatus.Up;
                var isNotLoopback = network.NetworkInterfaceType != NetworkInterfaceType.Loopback;
                var isGateway = network.GetIPProperties()?.GatewayAddresses.Any(c => c?.Address != null) ?? false;
                return isUp && isNotLoopback && isGateway;
            })
            .Select(n => new
            {
                IPAddress = n.GetIPProperties().UnicastAddresses.FirstOrDefault(c => c.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.Address,
                Gateway = n.GetIPProperties()?.GatewayAddresses.FirstOrDefault()?.Address,
            }).FirstOrDefault();

        if (networkInterface != null)
        {
            return networkInterface.IPAddress!;
        }

        var hostname = Dns.GetHostName();
        var ipEntry = Dns.GetHostEntry(hostname);
        var ipAddress = ipEntry.AddressList[1];
        return ipAddress;
    }
}