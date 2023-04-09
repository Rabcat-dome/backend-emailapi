namespace PTTDigital.Email.Common.Configuration.AppSetting.API.AuthenticationConstant;

internal class AuthValue : IAuthValue
{
    public string? AuthScheme { get; init; }
    public string? Value { get; init; }
    public bool IsValid { get; init; }
    public bool IsGuid { get; init; }
}
