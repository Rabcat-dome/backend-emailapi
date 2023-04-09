namespace PTTDigital.Email.Common.Configuration.AppSetting.API.AuthenticationConstant;

public interface IAuthValue
{
    string? AuthScheme { get; }
    string? Value { get; }
    bool IsValid { get; }
    bool IsGuid { get; }
}
