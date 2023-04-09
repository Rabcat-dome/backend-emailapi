using PTTDigital.Email.Common.Configuration.AppSetting.API.BackendApi;

namespace PTTDigital.Email.Common.Configuration.AppSetting;

    public interface IApiConfigurationFactory
    {
        IApiConfiguration GetConfiguration(ApiClientType clientType);

        bool IsValidConfiguration(ApiClientType clientType);
}
