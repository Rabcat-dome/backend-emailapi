using PTTDigital.Email.Common.Configuration.AppSetting.API.BackendApi;
using PTTDigital.Email.Common.Configuration.AppSetting.API.Client;

namespace PTTDigital.Email.Common.Configuration.AppSetting;

public interface IAppSetting
{
    Dictionary<string, ApiConfiguration>? ApiClients { get; }

    ClientConfiguration ClientConfigs { get; }

    string? SymmetricKey { get; }
    string? OAuthSession { get; set; }

    string? GrapgApiUserProfile { get; }

    string? AppId { get; }

    string? AppName { get; }

    bool UseInMemory { get; }
    bool IsTest { get; set; }
    string? AdminEmail { get; set; }
    int SMTPPort { get; set; }
    string? SMTPServer { get; set; }
    string? DefaultMail { get; set; }
    string? DefaultMailDisplay { get; set; }
    string[]? AllowOrigins { get; }
    string? UserInGroup { get; }

    string? TbSysPermissions { get; }
}