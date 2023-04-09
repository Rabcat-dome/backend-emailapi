using PTTDigital.Email.Application.Models;

namespace PTTDigital.Email.Application.Services.Token;

public interface IOAuthUserService
{
    OAuthResultModel<OAuthUserModel> VerifyAccount(string email);
}
