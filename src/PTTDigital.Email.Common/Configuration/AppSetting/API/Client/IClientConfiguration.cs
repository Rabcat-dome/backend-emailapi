namespace PTTDigital.Email.Common.Configuration.AppSetting.API.Client;

public interface IClientConfiguration
{
    public string ClientId { get; }

    public string ClientSecret { get; }

    public int RefreshTokenLifeTimeInMinutes { get; set; }

    public int AccessTokenExpireInMinutes { get; set; }

    public int SessionTokenExpireInHour { get; set; }

    public string Password { get; set; }

    public bool IgnoreClientPassword { get; set; }

    public List<OAuthApiClientModel> OAuthApiClientModels { get; }
}
