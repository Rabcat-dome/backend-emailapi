using System.Text.RegularExpressions;

namespace PTTDigital.Email.Common.Configuration.AppSetting.API.AuthenticationConstant;

public static class EmailConstant
{
    public const string AccessToken = "as:accessToken";
    public const string InvalidClientError = "invalid_client";
    public const string InvalidGrantError = "[Custom]-invalid_grant";

    public const string AccessKey = "access_token:";
    public const string RefreshKey = "refresh_token:";
    public const string TokenKey = "smt_client_token:";

    public const string HttpHeaderSessionToken = "X-SessionToken";
    public const string HttpHeaderOriginHostname = "X-Origin-Hostname";
    public const string HttpHeaderResource = "X-Resource";
    public const string HttpHeaderForwardedFor = "X-Forwarded-For";
    public const string HttpHeaderProcessIDHostname = "X-PID";
    public const string HttpHeaderTraceId = "X-TraceId";
    public const string HttpHeaderAppId = "X-AppId";
    public const string HttpHeaderAppName = "X-AppName";


    private const string _scheme = "scheme";
    private const string _value = "value";
    private static readonly Regex regex = new(@$"(?<{_scheme}>\w+)\s(?<{_value}>[\w\W]+)", RegexOptions.IgnoreCase);

    public static IAuthValue GetAuthValue(string authorization)
    {
        if (string.IsNullOrWhiteSpace(authorization))
        {
            return default!;
        }

        // Request from Swagger
        if (Guid.TryParse(authorization, out var _))
        {
            return new AuthValue
            {
                AuthScheme = "Bearer",
                Value = authorization,
                IsValid = true,
                IsGuid = true,
            };
        }

        var match = regex.Match(authorization);
        if (!match.Success)
        {
            return default!;
        }

        var scheme = match.Groups[_scheme].Value;
        var value = match.Groups[_value].Value;
        var isGuid = !string.IsNullOrEmpty(value) && Guid.TryParse(value, out var _);
        var isValid = scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(value);

        return new AuthValue
        {
            AuthScheme = scheme,
            Value = value,
            IsValid = isValid,
            IsGuid = isGuid,
        };
    }
}
