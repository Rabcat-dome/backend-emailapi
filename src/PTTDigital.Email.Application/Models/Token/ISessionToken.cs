namespace PTTDigital.Email.Application.Models.Token;

public interface ISessionToken
{
    Task<SessionTokenModel> GetSessionTokenAsync(string accessToken);
}
