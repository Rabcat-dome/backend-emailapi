using PTTDigital.Email.Application.Models;
using PTTDigital.Email.Application.Models.Token;

namespace PTTDigital.Email.Application.Authorization;

public interface IApiAuthorizeServer : ISessionToken
{
    Task<TokenModel> GrantAuthenticationAsync(AuthorizationModel model);

    Task RevokeAuthenticationAsync(string accessToken);
}
