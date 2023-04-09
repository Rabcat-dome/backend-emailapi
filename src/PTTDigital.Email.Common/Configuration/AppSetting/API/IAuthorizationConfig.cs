namespace PTTDigital.Email.Common.Configuration.AppSetting.API;

public interface IAuthorizationConfig
{
    string? JwtSecret { get; }
}
