using System.Security.Claims;

namespace PTTDigital.Email.Common.ApplicationUser;

public interface IJwtManager
{
    string GenerateToken<TPayload>(TPayload payload, int expireInMinutes);

    ClaimsPrincipal GetClaimsPrincipal(string token);

    bool IsExpiredDate(ClaimsPrincipal claimsPrincipal);
}
