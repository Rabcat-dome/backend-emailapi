namespace PTTDigital.Email.Common.Helper.LogExtension;

public static class LogExtension
{
    private const string anonymousType = "AnonymousType";
    private const string messageFormat = "[Caller:{Caller}] [TraceId:{TraceId}] [DataType:{DataType}] [Message:{Message}]";
    private static readonly string[] maskingKeys = new[] { "password", "passcode", "secret" };

    public static void Log(this ILogger logger
        , LogLevel level, EventId eventId, object model
        , System.Exception exception = null!, [CallerMemberName] string caller = "")
    {
        IApplicationUser? appUser = default!;
        if (logger is CustomLogger custom)
        {
            appUser = custom?.ApplicationUser!;
        }

        logger.Log(level, eventId, model, appUser, exception, caller);
    }

    public static void Log(this ILogger logger
        , LogLevel level, EventId eventId, object model
        , IApplicationUser applicationUser
        , System.Exception exception = null!, [CallerMemberName] string caller = "")
    {
        var _logger = logger;
        var _level = level;
        var _eventId = eventId;
        var _model = model;
        var _appUser = applicationUser;
        var _exception = exception;
        var _caller = caller;

        Task.Factory.StartNew(() =>
        {
            var typeName = string.Empty;
            var message = string.Empty;

            if (_model is not null)
            {
                var type = _model.GetType();
                var isAnonymousType = CheckIfAnonymousType(type);
                typeName = isAnonymousType ? anonymousType : type.FullName;

                try
                {
                    if (isAnonymousType)
                    {
                        message = JsonHelper.SerializeObject(_model);
                    }
                    else if (type.Equals(typeof(string)))
                    {
                        message = _model.ToString();
                    }
                    else
                    {
                        var json = JsonHelper.SerializeObject(_model);
                        var clone = JsonConvert.DeserializeObject(json, type);
                        var obj = DataMasking(clone!);

                        message = obj is null ? null : JsonHelper.SerializeObject(obj);
                    }
                }
                catch
                {
                    message = _model.ToString();
                }
            }

            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["IPAddress"] = _appUser?.RemoteAddress ?? string.Empty,
            }))
            {
                _logger.Log(_level, _eventId, _exception, messageFormat
                    , _caller ?? string.Empty
                    , _appUser?.TraceIdentifier ?? string.Empty
                    , typeName ?? string.Empty
                    , message ?? string.Empty);
            }

        }, CancellationToken.None, TaskCreationOptions.RunContinuationsAsynchronously, TaskScheduler.Default);
    }

    /// <summary>
    /// Determining whether a Type is an Anonymous Type<br/>
    /// <see href="https://stackoverflow.com/questions/1650681/determining-whether-a-type-is-an-anonymous-type"/>
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    private static bool CheckIfAnonymousType(Type type)
    {
        ArgumentNullException.ThrowIfNull(nameof(type));

        // HACK: The only way to detect anonymous types right now.
        return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
            && type.IsGenericType && type.Name.Contains(anonymousType)
            && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
            && type.Attributes.HasFlag(TypeAttributes.NotPublic);
    }

    private static object DataMasking(object obj)
    {
        if (obj is null)
        {
            return null!;
        }

        var type = obj.GetType();
        var props = type.GetProperties()
                        .Where(c => c.CanRead && c.CanWrite
                            && c.PropertyType == typeof(string)
                            && maskingKeys.Any(keyword => c.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                        .ToList();

        foreach (var prop in props)
        {
            var val = prop.GetValue(obj)?.ToString()?.Length ?? 0;
            prop.SetValue(obj, $"[{val}]");
        }

        return obj;
    }
}
