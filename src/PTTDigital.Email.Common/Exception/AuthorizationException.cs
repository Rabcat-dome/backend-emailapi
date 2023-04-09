using System.Runtime.CompilerServices;

namespace PTTDigital.Email.Common.Exception;

public class AuthorizationException : System.Exception
{
    public ErrorModel Model { get; }

    public AuthorizationException(string message, [CallerMemberName] string memberName = "") : base(message)
    {
        Model = new ErrorModel
        {
            Error = memberName,
            Description = message,
        };
    }

    public AuthorizationException(ErrorModel model): base(model?.Description)
    {
        Model = model;
    }
}