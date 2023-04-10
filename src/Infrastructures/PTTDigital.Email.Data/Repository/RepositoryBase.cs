using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Caching.Memory;
using PTTDigital.Authentication.Data.Models;
using PTTDigital.Email.Application.Repositories;
using PTTDigital.Email.Data.Paging;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace PTTDigital.Email.Data.Repository;

public abstract class RepositoryBase<TEntity, TDbContext> : IRepository<TEntity>
    where TEntity : class
    where TDbContext : DbContext
{
    protected readonly TDbContext context;
    private readonly IGenerator generator;
    protected readonly DbSet<TEntity> dataSet;
    protected readonly Type entityType;
    protected readonly IMemoryCache memoryCache;
    protected readonly string contextName;
    protected readonly TableAttribute Table;

    protected readonly bool isIAudit = typeof(IAudit).IsAssignableFrom(typeof(TEntity));
    protected readonly bool isIEntity = typeof(IEntity).IsAssignableFrom(typeof(TEntity));


    //protected RepositoryBase(TDbContext context)
    //{
    //    this.context = context;
    //    this.dataSet = this.context.Set<TEntity>();
    //}

    protected RepositoryBase(TDbContext db, IGenerator generator, IMemoryCache cache)
    {
        this.context = db;
        this.generator = generator;
        this.memoryCache = cache;
        this.dataSet = this.context.Set<TEntity>();
        //this.entityType = typeof(TEntity);
        //this.contextName = typeof(TDbContext).Name;
        //this.Table = this.entityType.GetCustomAttributes(true).OfType<TableAttribute>().FirstOrDefault();
    }

    public Task<bool> AnyAsync()
    {
        return dataSet.AnyAsync();
    }

    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return dataSet.AnyAsync();
    }

    public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return dataSet.Where(predicate).FirstOrDefaultAsync();
        //var xx = dataSet.FindAsync();
    }

    public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate)
    {
        if (predicate == null)
        {
            return dataSet.AsNoTracking();
        }
        else
        {
            return dataSet.AsNoTracking().Where(predicate);
        }
    }

    public async Task<ResponsePagination<TEntity>> QueryPagingAsync(IPagination pagination)
    {
        ArgumentNullException.ThrowIfNull(pagination);

        var predicate = pagination.GetPredicate<TEntity>();
        var query = Query(predicate);
        var totalRecord = await query.CountAsync();

        var result = pagination.SortTypes switch
        {
            SortTypes.Desc => query.OrderByDescending(pagination.OrderBy),
            _ => query.OrderBy(pagination.OrderBy)
        };

        var data = await result
            .Skip(pagination.PageNumberSize())
            .Take(pagination.PageSize)
            .ToListAsync();

        var response = new ResponsePagination<TEntity>()
        {
            TotalRecord = totalRecord,
            Data = data
        };

        return response;
    }

    public EntityEntry<TEntity> Add(TEntity entity)
    {
        if (isIAudit && isIEntity)
        {
            var dateTime = DateTime.Now;
            var tmpEntity = (IEntity)entity;
            var tmpAudit = (IAudit)entity;
            //tmpEntity.Id = generator.GenerateUlid();
            //tmpEntity.IsActive = true;
            //tmpEntity.IsDeleted = false;
            //tmpAudit.CreatedDate = dateTime;
            //tmpAudit.UpdatedDate = dateTime;
            //tmpAudit.UpdatedBy = null;
            //tmpAudit.DeletedDate = null;
            //tmpAudit.DeletedBy = null;
        }

        return dataSet.Add(entity);
    }

    public EntityEntry<TEntity> Add(TEntity entity, bool setIsActive)
    {
        if (isIAudit && isIEntity)
        {
            var tmpEntity = (IEntity)entity;
            var tmpAudit = (IAudit)entity;
            //tmpEntity.Id = generator.GenerateUlid();
            //tmpEntity.IsDeleted = false;
            //tmpAudit.CreatedDate = DateTime.Now;
            //tmpAudit.UpdatedDate = null;
            //tmpAudit.UpdatedBy = null;
            //tmpAudit.DeletedDate = null;
            //tmpAudit.DeletedBy = null;
        }

        return dataSet.Add(entity);
    }

    public EntityEntry<TEntity> Update(TEntity entity)
    {
        context.Entry(entity).State = EntityState.Detached;

        if (isIAudit && isIEntity)
        {
            var tmpAudit = (IAudit)entity;
            //tmpAudit.UpdatedDate = DateTime.Now;
        }

        return dataSet.Update(entity);
    }

    public EntityEntry<TEntity> Remove(TEntity entity)
    {
        if (isIAudit && isIEntity)
        {
            var tmpEntity = (IEntity)entity;
            var tmpAudit = (IAudit)entity;
            //tmpEntity.IsDeleted = true;
            //tmpAudit.DeletedDate = DateTime.Now;
        }

        return dataSet.Update(entity);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        dataSet.AddRange(entities);
    }

    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        dataSet.UpdateRange(entities);
    }

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        dataSet.RemoveRange(entities);
    }
}