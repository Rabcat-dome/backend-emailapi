namespace PTTDigital.Email.Common.Configuration.AppSetting;

public interface IKeyVaultSettings
{
    string? Url { get; }
    string? TenantId { get; }
    string? ClientId { get; }
    string? ClientSecret { get; }
}