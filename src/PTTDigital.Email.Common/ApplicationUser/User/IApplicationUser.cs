namespace PTTDigital.Email.Common.ApplicationUser.User;

public interface IApplicationUser
{
    string TraceIdentifier { get; }

    string UserId { get; }

    string UserName { get; }

    string SessionToken { get; }

    string RemoteAddress { get; }

    string TraceId { get; }

    string AppId { get; }

    public string AppName { get; }

    public string? AccessToken { get; }
}
