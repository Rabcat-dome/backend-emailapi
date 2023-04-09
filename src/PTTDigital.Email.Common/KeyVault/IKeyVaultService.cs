namespace PTTDigital.Email.Common.KeyVault;

public interface IKeyVaultService
{
    bool IsValid { get; }

    string GetKeyVaultValue(string secretName);
}
    