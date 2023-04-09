namespace PTTDigital.Email.Application.Repositories.Token;

[Obsolete(".Net Framework is obsolete")]
public static class AccessTokenStore
{
    private static ConcurrentDictionary<string, string> _accessTokens = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

    public static bool Add(string token, string protectedTicket)
    {
        return _accessTokens.TryAdd(token, protectedTicket);
    }

    public static bool Revoke(string token)
    {
        return _accessTokens.TryRemove(token, out string value);
    }

    public static bool GetValue(string token, out string value)
    {
        return _accessTokens.TryGetValue(token, out value);
    }
}