namespace PTTDigital.Email.Common.Configuration.AppSetting.API.BackendApi;

public class ApiConfiguration : IApiConfiguration
{
    public string? BaseUri { get; set; }

    public string? ApiKey { get; set; }
}
