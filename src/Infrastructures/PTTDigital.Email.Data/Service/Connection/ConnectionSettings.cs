using PTTDigital.Email.Common.KeyVault;

namespace PTTDigital.Email.Data.Service.Connection;

public abstract class ConnectionSettings : IConnectionSettings
{
    protected readonly IKeyVaultService keyVaultService;

    protected ConnectionSettings(IKeyVaultService keyVaultService)
    {
        this.keyVaultService = keyVaultService;
    }
    public string ConnectionName { get; internal set; }

    public string AppName { get; internal set; }

    public string Host { get; internal set; }

    public int Port { get; internal set; }

    public string Database { get; internal set; }

    public int ConnectionTimeout { get; internal set; }

    public string Credentials { get; internal set; }

    public string ConnectionPattern { get; internal set; }

    public string? AzureKeyVaultUrl { get; internal set; }

    public string? AzureKeyVaultTenantId { get; internal set; }

    public string? AzureKeyVaultClientId { get; internal set; }

    public string? AzureKeyVaultClientSecret { get; internal set; }

    public abstract string GetConnectionString();
}