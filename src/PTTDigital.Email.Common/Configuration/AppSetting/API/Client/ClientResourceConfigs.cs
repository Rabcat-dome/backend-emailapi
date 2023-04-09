namespace PTTDigital.Email.Common.Configuration.AppSetting.API.Client;

public class ClientResourceConfigs : IClientConfigs
{
    private readonly IAppSetting _appSetting;
    public ClientResourceConfigs(IAppSetting appSetting)
    {
        _appSetting = appSetting;
    }

    public List<OAuthApiClientModel> ClientConfigs { get; set; }

    OAuthApiClientModel IClientConfigs.GetClientId(string clientId)
    {
        ClientConfigs = _appSetting.ClientConfigs.OAuthApiClientModels;

        return ClientConfigs?.FirstOrDefault(c => c.ClientId.Equals(clientId));
    }
}
