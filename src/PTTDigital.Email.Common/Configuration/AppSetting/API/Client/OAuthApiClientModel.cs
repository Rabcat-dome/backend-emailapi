namespace PTTDigital.Email.Common.Configuration.AppSetting.API.Client;

public class OAuthApiClientModel
{
    private int sessionTokenExpireInHour = 0;

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public int RefreshTokenLifeTimeInMinutes { get; set; }

    public int AccessTokenExpireInMinutes { get; set; }

    public int SessionTokenExpireInHour
    {
        get => sessionTokenExpireInHour < 1 ? 24 : sessionTokenExpireInHour;
        set => sessionTokenExpireInHour = value;
    }

    public string Password { get; set; }

    public bool IgnoreClientPassword { get; set; }
}
