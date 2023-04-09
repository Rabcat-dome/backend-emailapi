using PTTDigital.Email.Application.Models;

namespace PTTDigital.Email.Application.Utility;

public static class ResultMessage<T>
{
    public static OAuthResultModel<T> Error(string displayMessage)
    {
        var result = new OAuthResultModel<T>
        {
            Success = false,
            DisplayMessage = displayMessage
        };
        return result;
    }

    public static OAuthResultModel<T> Error(string displayMessage, string internalMessage)
    {
        var result = new OAuthResultModel<T>
        {
            Success = false,
            DisplayMessage = displayMessage,
            InternalMessage = internalMessage
        };
        return result;
    }

    public static OAuthResultModel<T> ExceptionError(Exception ex)
    {
        var result = new OAuthResultModel<T>
        {
            Success = false,
            DisplayMessage = ex.Message,
            InternalMessage = ex.ToString()
        };
        return result;
    }

    public static OAuthResultModel<T> Success(T value)
    {
        var result = new OAuthResultModel<T>
        {
            Success = true,
            Value = value
        };
        return result;
    }

    public static OAuthResultModel<T> Success(T value, string message)
    {
        var result = new OAuthResultModel<T>
        {
            Success = true,
            DisplayMessage = message,
            Value = value
        };

        return result;
    }
}
