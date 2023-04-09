namespace PTTDigital.Authentication.Common;

public class ResultModel
{
    public bool Success { get; set; }

    public string DisplayMessage { get; set; }

    public string InternalMessage { get; set; }
}

public class ResultModel<T> : ResultModel
{
    public T Value { get; set; }
}
