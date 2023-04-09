using System.Runtime.CompilerServices;

namespace PTTDigital.Email.Common.Exception;

public class BadRequestException : System.Exception
{
    public ErrorModel Model { get; }

    public BadRequestException(string message, [CallerMemberName] string memberName = "") : base(message)
    {
        Model = new ErrorModel
        {
            Error = memberName,
            Description = message,
        };
    }

    public BadRequestException(ErrorModel model) : base(model?.Description)
    {
        Model = model;
    }
}
