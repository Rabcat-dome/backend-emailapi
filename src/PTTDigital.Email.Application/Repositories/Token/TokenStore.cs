using System.Reflection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using PTTDigital.Email.Application.Repositories.Redis;
using PTTDigital.Email.Application.Repositories.Token.TokenKeyPair;
using PTTDigital.Email.Common.Helper;
using PTTDigital.Email.Common.Helper.LogExtension;
using PTTDigital.Email.Common.Redis;

namespace PTTDigital.Email.Application.Repositories.Token;

internal class TokenStoreRedis : ITokenStore    
{
    private readonly IRedisService _redisService;
    private readonly ILoggerHelper _loggerHelper;

    public TokenStoreRedis(IRedisService redisService, ILoggerHelper loggerHelper)
    {
        this._redisService = redisService;
        this._loggerHelper = loggerHelper;
    }

    public async Task<int> SaveAsync(IEnumerable<ITokenKeyPair> tokenKeys)
    {
        var models = tokenKeys.Select(item => (item as TokenKeyPair.TokenKeyPair)?.GetRedisModel())
                             .Where(c => c is not null);

        var logger = _loggerHelper.CreateLogger(GetType());
        LogExtension.Log(logger, LogLevel.Debug, LogEvents.InternalService, models);

        var result = await _redisService.SaveAsync(models);
        return result.Value;
    }

    public async Task AddAsync(string key, string value, double expirationInMinutes)
    {
        var model = new RedisSetModel
        {
            Key = key,
            ExpirationInMinutes = expirationInMinutes,
            Value = new List<dynamic>(new[] { value } )
        };

        var logger = _loggerHelper.CreateLogger(GetType());
        LogExtension.Log(logger, LogLevel.Debug, LogEvents.InternalService, model);

        await _redisService.SetAsync(model);
    }

    public async Task<string> GetAsync(string key)
    {
        var model = new RedisGetModel { Key = key };

        var logger = _loggerHelper.CreateLogger(GetType());
        LogExtension.Log(logger, LogLevel.Debug, LogEvents.InternalService, model);

        var result = await _redisService.GetAsync<string[]>(model);
        return result?.Value?.FirstOrDefault();
    }

    public async Task<TResult> SearchAsync<TResult>(string pattern) where TResult : class
    {
        var searchModel = new RedisSearchModel { Pattern = pattern };

        var logger = _loggerHelper.CreateLogger(GetType());
        LogExtension.Log(logger, LogLevel.Debug, LogEvents.InternalService, searchModel);

        var searchResult = await _redisService.SearchAsync<TResult>(searchModel);
        var result = searchResult?.Value?.FirstOrDefault();

        return result;
    }

    public async Task RemoveAsync(string key)
    {
        var model = new RedisDeleteModel { Key = key };
        await _redisService.DeleteAsync(model);
    }
}

internal class TokenStoreMemory : ITokenStore
{
    private readonly IMemoryCache memoryCache;
    private readonly ILoggerHelper loggerHelper;

    public TokenStoreMemory(IMemoryCache memoryCache, ILoggerHelper loggerHelper)
    {
        this.memoryCache = memoryCache;
        this.loggerHelper = loggerHelper;
    }

    public async Task<int> SaveAsync(IEnumerable<ITokenKeyPair> tokenKeys)
    {
        var models = tokenKeys.Select(item => (item as TokenKeyPair.TokenKeyPair)?.GetRedisModel())
                             .Where(c => c is not null);

        foreach (var model in models.OfType<RedisDeleteModel>())
        {
            if (model.HasPattern)
            {
                var keys = SearchKeys(model.Key);
                keys.ForEach(key => memoryCache.Remove(key));
            }
            else
            {
                memoryCache.Remove(model.Key);
            }
        }

        foreach (var model in models.OfType<RedisSetModel>())
        {
            var value = model.Value?.FirstOrDefault();
            var json = JsonHelper.SerializeObject(value);
            await AddAsync(model.Key, json, model.ExpirationInMinutes);
        }

        return models.Count();
    }

    public Task AddAsync(string key, string value, double expirationInMinutes)
    {
        try
        {
            var expirationTime = DateTime.Now.AddMinutes(expirationInMinutes);
            var expirationToken = new CancellationChangeToken(
                new CancellationTokenSource(TimeSpan.FromMinutes(expirationInMinutes + .01)).Token);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                 // Pin to cache.
                 .SetPriority(CacheItemPriority.Normal)
                 // Set the actual expiration time
                 .SetAbsoluteExpiration(expirationTime)
                 // Force eviction to run
                 .AddExpirationToken(expirationToken)
                 // Add eviction callback
                 .RegisterPostEvictionCallback(callback: CacheItemRemoved, state: this);

            memoryCache.Set(key, value, cacheEntryOptions);

            return Task.CompletedTask;
        }
        catch
        {
            return Task.CompletedTask;
        }
    }

    private void CacheItemRemoved(object key, object value, EvictionReason reason, object state)
    {
        var logger = loggerHelper.CreateLogger(GetType());
        LogExtension.Log(logger, LogLevel.Debug, LogEvents.InternalService, new
        {
            Key = key,
            Value = value,
            Reason = reason,
        });
    }

    public Task<string> GetAsync(string key)
    {
        try
        {
            var result = memoryCache.Get(key)?.ToString();
            return Task.FromResult(result);
        }
        catch
        {
            return default;
        }
    }

    public async Task<TResult> SearchAsync<TResult>(string pattern) where TResult : class
    {
        var keys = SearchKeys(pattern);
        var key = keys.FirstOrDefault();

        if (string.IsNullOrEmpty(key))
        {
            return default;
        }

        var value = await GetAsync(key);

        if (string.IsNullOrEmpty(value))
        {
            return default;
        }

        var result = JsonHelper.DeserializeObject<TResult>(value);
        return result;
    }

    public Task RemoveAsync(string key)
    {
        try
        {
            memoryCache.Remove(key);
            return Task.CompletedTask;
        }
        catch
        {
            return Task.CompletedTask;
        }
    }

    private List<string> SearchKeys(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            return new List<string>();
        }
        
        var keys = GetAllKeysList(memoryCache);
        var startValue = string.Empty;
        var endValue = string.Empty;

        if (pattern.StartsWith('*') && !pattern.EndsWith('*'))
        {
            endValue = pattern.TrimStart('*')!;
        }
        else if (pattern.EndsWith('*') && !pattern.StartsWith('*'))
        {
            startValue = pattern.TrimEnd('*')!;
        }
        else if (pattern.Contains('*') && !pattern.StartsWith('*') && !pattern.EndsWith('*'))
        {
            var values = pattern.Split('*');
            startValue = values.FirstOrDefault()!;
            endValue = values.LastOrDefault()!;
        }
        else
        {
            startValue = pattern;
        }

        var result = keys.Where(key =>
        {
            if (!string.IsNullOrEmpty(startValue) && !string.IsNullOrEmpty(endValue))
            {
                return key.StartsWith(startValue) && key.EndsWith(endValue);
            }
            else if (!string.IsNullOrEmpty(startValue))
            {
                return key.StartsWith(startValue);
            }
            else if (!string.IsNullOrEmpty(endValue))
            {
                return key.EndsWith(endValue);
            }
            else
            {
                return false;
            }
        }).ToList();

        return result;
    }

    /// <summary>
    /// Getting all cache keys Microsoft.Extensions.Caching.Memory
    /// <see href="https://github.com/dotnet/runtime/issues/36026"/>
    /// </summary>
    /// <returns></returns>
    private static List<string> GetAllKeysList(IMemoryCache memoryCache)
    {
        var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
        var collection = field.GetValue(memoryCache) as ICollection;
        var items = new List<string>();
        if (collection != null)
            foreach (var item in collection)
            {
                var methodInfo = item.GetType().GetProperty("Key");
                var val = methodInfo.GetValue(item);
                items.Add(val.ToString());
            }
        return items;
    }
}