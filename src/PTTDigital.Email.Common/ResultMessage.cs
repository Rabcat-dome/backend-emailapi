namespace PTTDigital.Email.Common;

public class ResultMessage
{
    public static ResultModel Error(string displayMessage)
    {
        var result = new ResultModel
        {
            Success = false,
            DisplayMessage = displayMessage
        };

        return result;
    }

    /// <summary>
    /// Apply for Credit calculation only !!!!!!!!!!!!!!!!!!!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="displayMessage"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static ResultModel Error<T>(string displayMessage, T value)
    {
        var result = new ResultModel<T>
        {
            Success = false,
            Value = value,
            DisplayMessage = displayMessage
        };
        return result;
    }

    public static ResultModel Error(string displayMessage, string internalMessage)
    {
        var result = new ResultModel
        {
            Success = false,
            DisplayMessage = displayMessage,
            InternalMessage = internalMessage
        };

        return result;
    }

    public static ResultModel ExceptionError(System.Exception ex)
    {
        var result = new ResultModel
        {
            Success = false,
            InternalMessage = ex?.ToString()
        };

        return result;
    }

    public static ResultModel Success()
    {
        var result = new ResultModel
        {
            Success = true
        };

        return result;
    }
}

public class ResultMessage<T>
{
    public static ResultModel<T> Error(string displayMessage)
    {
        var result = new ResultModel<T>
        {
            Success = false,
            DisplayMessage = displayMessage
        };

        return result;
    }

    /// <summary>
    /// Apply for Credit calculation only !!!!!!!!!!!!!!!!!!!
    /// </summary>
    /// <param name="displayMessage"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static ResultModel<T> Error(string displayMessage, T value)
    {
        var result = new ResultModel<T>
        {
            Success = false,
            DisplayMessage = displayMessage,
            Value = value
        };
        return result;
    }

    /// <summary>
    /// Apply for Credit calculation only !!!!!!!!!!!!!!!!!!!
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static ResultModel<T> Error(T value)
    {
        var result = new ResultModel<T>
        {
            Success = false,
            Value = value
        };
        return result;
    }

    public static ResultModel<T> Error(string displayMessage, string internalMessage)
    {
        var result = new ResultModel<T>
        {
            Success = false,
            DisplayMessage = displayMessage,
            InternalMessage = internalMessage
        };

        return result;
    }

    public static ResultModel<T> ExceptionError(System.Exception ex)
    {
        var result = new ResultModel<T>
        {
            Success = false,
            InternalMessage = ex?.ToString()
        };

        return result;
    }

    public static ResultModel<T> Success(T value)
    {
        var result = new ResultModel<T>
        {
            Success = true,
            Value = value
        };

        return result;
    }
}
