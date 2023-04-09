namespace PTTDigital.Email.Api.Provider.Dependency;

public class JwtTokenConfig
{
    /// <remarks />
    public string Secret { get; set; }

    /// <remarks />
    public string Issuer { get; set; }

    /// <remarks />
    public string Audience { get; set; }

    /// <remarks />
    public int AccessTokenExpiration { get; set; }

    /// <remarks />
    public int RefreshTokenExpiration { get; set; }
}
