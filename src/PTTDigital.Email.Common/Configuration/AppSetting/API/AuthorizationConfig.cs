namespace PTTDigital.Email.Common.Configuration.AppSetting.API;

 public class AuthorizationConfig : IAuthorizationConfig
{
    public string? JwtSecret { get; set; }
}
