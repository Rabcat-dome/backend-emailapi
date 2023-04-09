namespace PTTDigital.Email.Common.Configuration.AppSetting.API.BackendApi
{
    public interface IApiConfiguration
    {
        string? ApiKey { get; set; }

        string? BaseUri { get; set; }
    }
}