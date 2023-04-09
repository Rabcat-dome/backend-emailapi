using PTTDigital.Email.Common.Configuration.AppSetting;

namespace PTTDigital.Email.Common.KeyVault;
public class KeyVaultService : IKeyVaultService
{
    private readonly IKeyVaultSettings keyVaultSettings;

    public KeyVaultService(IKeyVaultSettings keyVaultSettings)
    {
        this.keyVaultSettings = keyVaultSettings;
    }

    public bool IsValid => throw new NotImplementedException();

    public string GetKeyVaultValue(string secretName)
    {
        if (keyVaultSettings is null)
            return string.Empty;

        var tenantId = keyVaultSettings.TenantId;
        var clientId = keyVaultSettings.ClientId;
        var clientSecret = keyVaultSettings.ClientSecret;
        var url = keyVaultSettings.Url!;

        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        var client = new SecretClient(new Uri(url), credential);
        var secret = client.GetSecret(secretName);
        string secretValue = secret.Value.Value;
        return secretValue;
    }
}
