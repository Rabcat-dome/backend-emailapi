namespace PTTDigital.Email.Common.Redis;

public class RedisDeleteModel : IRedisModel
{
    public string? Key { get; set; }

    public bool HasPattern => !string.IsNullOrEmpty(Key) && Key.Contains('*');
}
