using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using PTTDigital.Email.Common.Configuration.AppSetting;

namespace PTTDigital.Email.Common.KeyVault;
public class KeyVaultService : IKeyVaultService
{
    private readonly IKeyVaultSettings _keyVaultSettings;

    public KeyVaultService(IKeyVaultSettings keyVaultSettings)
    {
        this._keyVaultSettings = keyVaultSettings;
    }

    public bool IsValid => throw new NotImplementedException();

    public string GetKeyVaultValue(string secretName)
    {
        if (_keyVaultSettings is null)
            return string.Empty;

        var tenantId = _keyVaultSettings.TenantId;
        var clientId = _keyVaultSettings.ClientId;
        var clientSecret = _keyVaultSettings.ClientSecret;
        var url = _keyVaultSettings.Url!;

        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        var client = new SecretClient(new Uri(url), credential);
        var secret = client.GetSecret(secretName);
        string secretValue = secret.Value.Value;
        return secretValue;
    }
}
