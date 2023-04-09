namespace PTTDigital.Email.Common.Configuration.AppSetting;

public class KeyVaultSettings : IKeyVaultSettings
{
    public string? Url { get; set; }
    public string? TenantId { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
}

