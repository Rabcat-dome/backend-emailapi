using PTTDigital.Email.Common.Configuration.AppSetting.API.BackendApi;

namespace PTTDigital.Email.Common.Configuration.AppSetting;

public class ApiConfigurationFactory : IApiConfigurationFactory
{
    private readonly IAppSetting _appSettings;

    public ApiConfigurationFactory(IAppSetting appSettings)
    {
        this._appSettings = appSettings;
    }

    public IApiConfiguration GetConfiguration(ApiClientType clientType)
    {
        var key = clientType.ToString();
        if (_appSettings.ApiClients!.TryGetValue(key, out ApiConfiguration configuration))
        {
            return configuration;
        }
        else
        {
            return null;
        }
    }

    bool IApiConfigurationFactory.IsValidConfiguration(ApiClientType clientType) => !string.IsNullOrWhiteSpace(GetConfiguration(clientType)?.BaseUri);
}
