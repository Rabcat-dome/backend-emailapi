namespace PTTDigital.Email.Common.Configuration.AppSetting.API.Client;

public class ClientConfiguration : IClientConfiguration
{
    public string ClientId => "PTTDigital.Authentication.Api";

    public string ClientSecret => "RGV2X1Rlc3Q=";
    public int RefreshTokenLifeTimeInMinutes { get; set; }

    public int AccessTokenExpireInMinutes { get; set; }

    public int SessionTokenExpireInHour { get; set; }

    public string Password { get; set; }

    public bool IgnoreClientPassword { get; set; }

    public List<OAuthApiClientModel> OAuthApiClientModels
    {
        get
        {
            return new List<OAuthApiClientModel>()
            {
                new OAuthApiClientModel()
                {
                    ClientId = ClientId,
                    ClientSecret = ClientSecret,
                    RefreshTokenLifeTimeInMinutes = RefreshTokenLifeTimeInMinutes,
                    AccessTokenExpireInMinutes = AccessTokenExpireInMinutes,
                    SessionTokenExpireInHour = SessionTokenExpireInHour,
                    Password = Password,
                    IgnoreClientPassword = IgnoreClientPassword
                }
            };
        }
    }
}