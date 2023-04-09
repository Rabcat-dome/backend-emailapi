using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PTTDigital.Email.Common.ApplicationUser;
using PTTDigital.Email.Common.Configuration.AppSetting.API;
using ServiceStack.DataAnnotations;

namespace PTTDigital.Email.Application.Services.Token;

internal class JwtAuthService : JwtManager
{
    private readonly IAuthorizationConfig authorizationConfig;

    public JwtAuthService(IAuthorizationConfig authorizationConfig)
    {
        this.authorizationConfig = authorizationConfig;
    }

    protected override string GenerateToken<TPayload>(TPayload payload, int expireInMinutes)
    {
        var secret = authorizationConfig.JwtSecret;
        var key = Encoding.UTF8.GetBytes(secret);
        if (key.Length < 16)
        {
            Array.Resize(ref key, 16);
        }

        var claimsCollection = GetPayloads(payload);
        var jwtPayload = new JwtPayload(issuer: string.Empty
            , audience: string.Empty
            , claims: null
            , claimsCollection: claimsCollection
            , notBefore: DateTime.UtcNow.ToLocalTime().AddSeconds(-1)
            , expires: DateTime.UtcNow.ToLocalTime().AddMinutes(expireInMinutes)
            , issuedAt: null);

        var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature);
        var header = new JwtHeader(credentials);
        var secToken = new JwtSecurityToken(header, jwtPayload);
        var jwtToken = new JwtSecurityTokenHandler().WriteToken(secToken);

        return jwtToken;
    }

    protected override ClaimsPrincipal GetClaimsPrincipal(string token)
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
                return new ClaimsPrincipal();

            var secret = authorizationConfig.JwtSecret;
            var validationParameters = TokenValidationSecretParameters(secret);
            result = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken _);
            return result;
        }
        catch
        {
            return result;
        }
    }

    private static TokenValidationParameters TokenValidationSecretParameters(string secret)
    {
        var key = Encoding.UTF8.GetBytes(secret);
        if (key.Length < 16)
        {
            Array.Resize(ref key, 16);
        }

        return new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateActor = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RequireExpirationTime = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    }

    private static Dictionary<string, object> GetPayloads<TPayload>(TPayload payload)
    {
        var acceptTypes = new[] { typeof(string), typeof(int), typeof(string[]), typeof(bool) };
        var result = new Dictionary<string, object>();
        var props = typeof(TPayload).GetProperties().Where(propInfo => propInfo.CanRead && acceptTypes.Contains(propInfo.PropertyType));

        foreach (var prop in props)
        {
            var name = GetDescription(prop) ?? prop.Name.ToLowerInvariant();

            var value = prop.GetValue(payload);
            if (value is not null && !result.ContainsKey(name))
            {
                result.Add(name, value);
            }
        }

        return result;
    }

    private static string GetDescription(PropertyInfo propInfo)
    {
        var attribute = (DescriptionAttribute)propInfo.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
        return attribute?.Description;
    }
}