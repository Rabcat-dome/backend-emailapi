using PTTDigital.Email.Common.Configuration.AppSetting.API;
using PTTDigital.Email.Common.Configuration.AppSetting.API.BackendApi;
using PTTDigital.Email.Common.Configuration.AppSetting.API.Client;
using PTTDigital.Email.Common.KeyVault;

namespace PTTDigital.Email.Common.Configuration.AppSetting;

public class AppSetting : IAppSetting
{
    private readonly IKeyVaultService keyVaultService;

    public AppSetting()
    {

    }
    public AppSetting(IKeyVaultService keyVaultService, string key)
    {
        this.keyVaultService = keyVaultService;
    }

    public Dictionary<string, ApiConfiguration>? ApiClients { get; set; }

    public AuthorizationConfig? Authorization { get; set; }

    public ClientConfiguration? ClientConfigs { get; set; }

    public string? SymmetricKey { get; set; }
    public string? OAuthSession { get; set; }

    public string? GrapgApiUserProfile { get; set; }

    public string? AppId { get; set; }

    public string? AppName { get; set; }

    public bool UseInMemory { get; set; }
    public bool IsTest { get; set; }
    public string? AdminEmail { get; set; }
    public int SMTPPort { get; set; } = 25;
    public string? SMTPServer { get; set; }
    public string? DefaultMail { get; set; }
    public string? DefaultMailDisplay { get; set; }
    public string[]? AllowOrigins { get; set; }

    public string? UserInGroup { get; set; }

    public string? TbSysPermissions { get; set; }

    private string GetKey(string value)
    {

        if (keyVaultService is null || !keyVaultService.IsValid)
        {
            return value;
        }

        return keyVaultService.GetKeyVaultValue(value);
    }
}
