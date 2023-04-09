using PTTDigital.Email.Application.Repositories.Token.TokenKeyPair;

namespace PTTDigital.Email.Application.Repositories.Token;

public interface ITokenStore
{
    Task<int> SaveAsync(IEnumerable<ITokenKeyPair> tokenKeys);
    Task AddAsync(string key, string value, double expirationInMinutes);
    Task<string> GetAsync(string key);
    Task<TResult> SearchAsync<TResult>(string pattern) where TResult : class;
    Task RemoveAsync(string key);
}
