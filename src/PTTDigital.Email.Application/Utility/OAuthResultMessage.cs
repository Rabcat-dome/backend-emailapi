using PTTDigital.Email.Application.Models;

namespace PTTDigital.Email.Application.Utility;

public static class OAuthResultMessage
{
    public static OAuthResultModel Error(string displayMessage)
    {
        var result = new OAuthResultModel
        {
            Success = false,
            DisplayMessage = displayMessage
        };
        return result;
    }

    public static OAuthResultModel Error(string displayMessage, string internalMessage)
    {
        var result = new OAuthResultModel
        {
            Success = false,
            DisplayMessage = displayMessage,
            InternalMessage = internalMessage
        };
        return result;
    }

    public static OAuthResultModel ExceptionError(Exception ex)
    {
        var result = new OAuthResultModel
        {
            Success = false,
            InternalMessage = ex?.ToString()
        };
        return result;
    }

    public static OAuthResultModel Success()
    {
        var result = new OAuthResultModel
        {
            Success = true
        };
        return result;
    }

    public static OAuthResultModel Success(string message)
    {
        var result = new OAuthResultModel
        {
            Success = true,
            DisplayMessage = message
        };

        return result;
    }
}