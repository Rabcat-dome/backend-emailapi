using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace PTTDigital.Email.Data.Service.Connection;

public static class AzureKeyVaultConfigurationExtensions
{
    public static string GetConnectionStringAzureKeyVault<TConnectionSettings>(this IConfiguration configuration, IServiceProvider service, string section, string name)
    where TConnectionSettings : ConnectionSettings
    {
        var configurationSection = configuration.GetSection(section);
        var settings = ConfigurationExtensions.GetConnectionSettings<TConnectionSettings>(configurationSection, service, section);

        ArgumentNullException.ThrowIfNull(settings);

        var result = settings.FirstOrDefault(c => c.ConnectionName.Equals(name));

        ArgumentNullException.ThrowIfNull(result);

        string mySecret = GetKeyVault(result.AzureKeyVaultUrl);

        return mySecret;
    }

    private static string GetKeyVault(string azureKeyVaultUrl)
    {
        //var tenantId = "cd01580f-b92e-494c-8b45-706ed5820613";
        //var clientId = "";
        //var clientSecret = "";
        //var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        var client = new SecretClient(new Uri(azureKeyVaultUrl), new DefaultAzureCredential());
        var secret = client.GetSecret("token-key");
        string secretValue = secret.Value.Value;
        return secretValue;
    }
}
