﻿using Microsoft.Extensions.Logging;

namespace PTTDigital.Email.Common.Helper.LogExtension;

public interface ILoggerHelper
{
    ILogger CreateLogger(Type type);
}
