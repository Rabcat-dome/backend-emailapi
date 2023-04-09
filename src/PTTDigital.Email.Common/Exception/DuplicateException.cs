﻿using System.Runtime.CompilerServices;

namespace PTTDigital.Email.Common.Exception;

public class DuplicateException : System.Exception
{
    public ErrorModel Model { get; }

    public DuplicateException(string message, [CallerMemberName] string memberName = "") : base(message)
    {
        Model = new ErrorModel
        {
            Error = memberName,
            Description = message,
        };
    }

    public DuplicateException(ErrorModel model) : base(model?.Description)
    {
        Model = model;
    }
}