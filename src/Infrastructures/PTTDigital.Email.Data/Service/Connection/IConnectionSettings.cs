namespace PTTDigital.Email.Data.Service.Connection;

public interface IConnectionSettings
{
    string ConnectionName { get; }

    string AppName { get; }

    string Host { get; }

    int Port { get; }

    string Database { get; }

    int ConnectionTimeout { get; }

    string Credentials { get; }

    string? AzureKeyVaultUrl { get; }

    string? AzureKeyVaultTenantId { get; }

    string? AzureKeyVaultClientId { get; }

    string? AzureKeyVaultClientSecret { get; }

    string GetConnectionString();
}