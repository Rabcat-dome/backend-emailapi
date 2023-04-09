using PTTDigital.Authentication.Common;
using PTTDigital.Email.Common.Redis;

namespace PTTDigital.Email.Application.Repositories.Redis;

public interface IRedisService
{
    Task<ResultModel<T>> GetAsync<T>(RedisGetModel model);
    Task<ResultModel<T[]>> SearchAsync<T>(RedisSearchModel model);
    Task<ResultModel<bool>> SetAsync(RedisSetModel model);
    Task<ResultModel<int>> SaveAsync(IEnumerable<IRedisModel> models);
    Task<ResultModel<bool>> DeleteAsync(RedisDeleteModel model);
    Task<ResultModel<bool>> DeleteAllAsync();
}
