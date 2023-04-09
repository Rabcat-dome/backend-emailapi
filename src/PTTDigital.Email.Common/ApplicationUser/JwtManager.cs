using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace PTTDigital.Email.Common.ApplicationUser;

public class JwtManager : IJwtManager
{
    string IJwtManager.GenerateToken<TPayload>(TPayload payload, int expireInMinutes)
    {
        return GenerateToken(payload, expireInMinutes);
    }

    ClaimsPrincipal IJwtManager.GetClaimsPrincipal(string token)
    {
        return GetClaimsPrincipal(token);
    }

    bool IJwtManager.IsExpiredDate(ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal is null || !claimsPrincipal.Claims.Any())
        {
            return true;
        }

        var payload = new JwtPayload(claimsPrincipal.Claims);
        var expDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(payload.Exp ?? 0).ToLocalTime();
        var nbfDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(payload.Nbf ?? 0).ToLocalTime();

        return (nbfDate > DateTime.Now) || (DateTime.Now > expDate);
    }

    protected virtual string GenerateToken<TPayload>(TPayload payload, int expireInMinutes)
    {
        return default;
    }

    protected virtual ClaimsPrincipal GetClaimsPrincipal(string token)
    {
        var result = new ClaimsPrincipal();
        try
        {
            if (string.IsNullOrWhiteSpace(token))
                return result;

            var tokenHandler = new JwtSecurityTokenHandler();
            var isJwt = tokenHandler.CanReadToken(token);
            if (!isJwt)
                return result;

            if (tokenHandler.ReadToken(token) is not JwtSecurityToken _)
                return result;

            var validationParameters = TokenValidationParameters();
            result = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken _);
            return result;
        }
        catch
        {
            return result;
        }
    }

    private static TokenValidationParameters tokenValidation = null;

    public static TokenValidationParameters TokenValidationParameters()
    {
        if (tokenValidation is null)
        {
            tokenValidation = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateActor = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = false,
                RequireExpirationTime = true,
                SignatureValidator = (string token, TokenValidationParameters parameters) => new JwtSecurityToken(token),
            };
        }

        return tokenValidation;
    }
}