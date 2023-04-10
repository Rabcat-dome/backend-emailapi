using Microsoft.EntityFrameworkCore.ChangeTracking;
using PTTDigital.Email.Data.Paging;
using System.Linq.Expressions;

namespace PTTDigital.Email.Data.Repository;

public interface IRepository<TEntity> where TEntity : class
{
    Task<bool> AnyAsync();

    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

    Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

    IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate);

    Task<ResponsePagination<TEntity>> QueryPagingAsync(IPagination pagination);

    EntityEntry<TEntity> Add(TEntity entity);

    EntityEntry<TEntity> Add(TEntity entity, bool setIsActive);

    EntityEntry<TEntity> Update(TEntity entity);

    EntityEntry<TEntity> Remove(TEntity entity);

    void AddRange(IEnumerable<TEntity> entities);

    void UpdateRange(IEnumerable<TEntity> entities);

    void RemoveRange(IEnumerable<TEntity> entities);
}