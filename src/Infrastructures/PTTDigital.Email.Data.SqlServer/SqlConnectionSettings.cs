using System.Text;
using PTTDigital.Email.Common.KeyVault;
using PTTDigital.Email.Data.Service.Connection;

namespace PTTDigital.Email.Data.SqlServer;

public class SqlConnectionSettings : ConnectionSettings
{
    private readonly IKeyVaultService _keyVaultService;

    public SqlConnectionSettings(IKeyVaultService keyVaultService) : base(keyVaultService)
    {
        this._keyVaultService = keyVaultService;
    }
    public override string GetConnectionString()
    {
        string? credentialsText = null;

        if (Credentials.StartsWith("KEY-"))
        {
            var base64 = _keyVaultService.GetKeyVaultValue(Credentials);
            var base64Bytes = Convert.FromBase64String(base64);
            credentialsText = Encoding.UTF8.GetString(base64Bytes);
        }
        else
        {
            credentialsText = Credentials;
        }

        var credentials = credentialsText.Split(':');
        var userName = credentials[0];
        var password = credentials[1];

        if (string.IsNullOrEmpty(AppName) || string.IsNullOrEmpty(Database) ||
            string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(Host))
        {
            throw new ArgumentNullException();
        }

        var connectionString = string.Format(ConnectionPattern, AppName, Host, Database, userName, password);
        return connectionString;
    }
}