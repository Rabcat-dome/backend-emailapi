namespace PTTDigital.Email.Application.Repositories.Token.TokenKeyPair;

public interface ITokenKeyPair
{
    StoreAction Action { get; }
    string Key { get; }
    object Value { get; }
    double ExpirationInMinutes { get; }
}
