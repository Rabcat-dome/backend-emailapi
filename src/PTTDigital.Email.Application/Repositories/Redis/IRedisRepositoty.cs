using PTTDigital.Email.Common.Redis;

namespace PTTDigital.Email.Application.Repositories.Redis;

public interface IRedisRepositoty
{
    Task<T> GetAsync<T>(RedisGetModel model);
    Task<T[]> SearchAsync<T>(RedisSearchModel model);
    Task<bool> SetAsync(RedisSetModel model);
    Task<int> SaveAsync(IEnumerable<IRedisModel> models);
    Task<bool> DeleteAsync(RedisDeleteModel model);
    Task<bool> DeleteAllAsync();
}
