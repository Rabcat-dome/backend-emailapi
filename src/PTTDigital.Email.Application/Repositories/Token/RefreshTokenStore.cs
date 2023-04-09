namespace PTTDigital.Email.Application.Repositories.Token;

[Obsolete(".Net Framework is obsolete")]
public static class RefreshTokenStore
{
    private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens = new ConcurrentDictionary<string, AuthenticationTicket>();

    public static bool Add(string token, AuthenticationTicket protectedTicket)
    {
        return _refreshTokens.TryAdd(token, protectedTicket);
    }

    public static bool Remove(string token, out AuthenticationTicket ticket)
    {
        return _refreshTokens.TryRemove(token, out ticket);
    }

    public static bool Revoke(string token)
    {
        var isRevoked = _refreshTokens.TryRemove(token, out AuthenticationTicket ticket);

        if (isRevoked)
        {
            AccessTokenStore.Revoke(ticket.Dictionary[AuthenticationConstant.AccessToken]);
        }

        return isRevoked;
    }
}