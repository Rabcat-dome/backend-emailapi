namespace PTTDigital.Email.Common.Redis;

public class RedisSetModel : IRedisModel
{
    public string? Key { get; set; }
    public List<dynamic>? Value { get; set; }
    public double ExpirationInMinutes { get; set; } = double.NaN;

    public bool HasExpiration => !double.IsNaN(ExpirationInMinutes) && (ExpirationInMinutes > 0);
    public TimeSpan ExpireIn => HasExpiration ? TimeSpan.FromMinutes(ExpirationInMinutes) : TimeSpan.Zero;
}
