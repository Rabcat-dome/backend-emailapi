using PTTDigital.Email.Application.Models;

namespace PTTDigital.Email.Application.Services.Token;

internal class OAuthUserService : IOAuthUserService
{
    private readonly IOAuthUserRepository _userRepository;

    public OAuthUserService(IOAuthUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public OAuthResultModel<OAuthUserModel> VerifyAccount(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return PTTDigital.Authentication.Application.Utility.ResultMessage<OAuthUserModel>.Error(Resources.EmailIsNullOrWhiteSpace);
            }

            var user = _userRepository.ValidateEmail(email);

            if (user == null)
            {
                return PTTDigital.Authentication.Application.Utility.ResultMessage<OAuthUserModel>.Error(Resources.LoginFailed);
            }

            return PTTDigital.Authentication.Application.Utility.ResultMessage<OAuthUserModel>.Success(user);
        }
        catch (CryptorEngineException)
        {
            return PTTDigital.Authentication.Application.Utility.ResultMessage<OAuthUserModel>.Error(Resources.DecryptPasswordFailed);
        }
        catch (Exception ex)
        {
            return PTTDigital.Authentication.Application.Utility.ResultMessage<OAuthUserModel>.ExceptionError(ex);
        }
    }
}