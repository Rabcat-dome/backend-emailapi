using PTTDigital.Email.Common.Helper;
using PTTDigital.Email.Common.Redis;
using ServiceStack.Redis;

namespace PTTDigital.Email.Application.Repositories.Redis;

public class RedisRepository : IRedisRepositoty
{
    private readonly string _prefix;
    private readonly IRedisClientsManagerAsync _managerAsync;

    public RedisRepository(IRedisClientsManagerAsync managerAsync)
    {
        _managerAsync = managerAsync;
        _prefix = _managerAsync.RedisResolver.PrimaryEndpoint.Client;
    }

    #region "Get"

    public async Task<T> GetAsync<T>(RedisGetModel model)
    {
        await using var redis = await _managerAsync.GetReadOnlyClientAsync();

        if (await redis.PingAsync())
        {
            var warpper = redis.As<T>();

            string key = $"{_prefix}:{model.Key}";

            var data = await warpper.GetValueAsync(key);

            return data;
        }

        return default!;
    }

    public async Task<T[]> SearchAsync<T>(RedisSearchModel model)
    {
        await using var redis = await _managerAsync.GetReadOnlyClientAsync();

        if (!await redis.PingAsync())
        {
            return default!;
        }

        var searcher = redis.As<string>();
        var pattern = $"{_prefix}:{model.Pattern}";
        var data = await searcher.SearchKeysAsync(pattern);
        var keys = data.OfType<string>().ToList() ?? Enumerable.Empty<string>();

        if (!keys.Any())
        {
            return default!;
        }

        var result = new List<T>();
        var warpper = redis.As<T>();

        foreach (var key in keys)
        {
            var value = await warpper.GetValueAsync(key);

            if (value is not null)
            {
                result.Add(value);
            }
        }

        return result.ToArray();
    }
    #endregion

    #region "Set"

    public async Task<bool> SetAsync(RedisSetModel model)
    {
        if (model?.Value is null) return false;

        await using var redis = await _managerAsync.GetClientAsync();

        if (!await redis.PingAsync())
        {
            return false;
        }

        string key = $"{_prefix}:{model.Key}";

        var json = JsonHelper.SerializeObject(model.Value);

        if (model.HasExpiration)
        {
            await redis.SetValueAsync(key, json, model.ExpireIn, CancellationToken.None);
        }
        else
        {
            await redis.SetValueAsync(key, json, CancellationToken.None);
        }

        return true;
    }

    public async Task<int> SaveAsync(IEnumerable<IRedisModel> models)
    {
        if (models is null || !models.Any())
            return 0;

        await using var redis = await _managerAsync.GetClientAsync();

        if (!await redis.PingAsync())
        {
            return 0;
        }

        var affected = 0;
        foreach (var model in models.OfType<RedisDeleteModel>())
        {
            if (await DeleteAsync(redis, model))
            {
                affected++;
            }
        }

        foreach (var model in models.OfType<RedisSetModel>())
        {
            if (await SetSingleValue(redis, model))
            {
                affected++;
            }
        }

        return affected;
    }

    private async Task<bool> SetSingleValue(IRedisClientAsync redis, RedisSetModel model)
    {
        if (model?.Value is null)
        {
            return false;
        }

        var key = $"{_prefix}:{model.Key}";
        var value = model.Value.FirstOrDefault();
        var json = JsonHelper.SerializeObject(value!);

        if (model.HasExpiration)
        {
            await redis.SetValueAsync(key, json, model.ExpireIn, CancellationToken.None);
        }
        else
        {
            await redis.SetValueAsync(key, json, CancellationToken.None);
        }

        return true;
    }
    #endregion

    #region "Delete"

    public async Task<bool> DeleteAsync(RedisDeleteModel model)
    {
        await using var redis = await _managerAsync.GetClientAsync();

        if (!await redis.PingAsync())
        {
            return false;
        }

        return await DeleteAsync(redis, model);
    }

    public async Task<bool> DeleteAllAsync()
    {
        await using var redis = await _managerAsync.GetClientAsync();

        if (await redis.PingAsync())
        {
            var keys = await redis.GetAllKeysAsync();

            await redis.RemoveAllAsync(keys, CancellationToken.None);
        }

        return true;
    }

    private async Task<bool> DeleteAsync(IRedisClientAsync redis, RedisDeleteModel model)
    {
        if (model is null)
        {
            return false;
        }

        string key = $"{_prefix}:{model.Key}";

        if (model.HasPattern)
        {
            await redis.RemoveByPatternAsync(key, CancellationToken.None);
            return true;
        }
        else
        {
            return await redis.RemoveAsync(key, CancellationToken.None);
        }
    }
    #endregion
}
