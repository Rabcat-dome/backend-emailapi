namespace PTTDigital.Email.Common.Configuration.AppSetting.API.Client;

public interface IClientConfigs
{
    OAuthApiClientModel GetClientId(string clientId);
}
