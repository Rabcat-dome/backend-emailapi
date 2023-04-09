using Microsoft.EntityFrameworkCore;

namespace PTTDigital.Email.Data.Service;

public abstract class DataServiceBase<TDbContext> where TDbContext : DbContext
{
    private readonly TDbContext context;

    protected DataServiceBase(TDbContext context)
    {
        this.context = context;
    }

    #region IServiceBase
    protected virtual async Task<int> SaveChangeAsync()
    {
        var affected = await context.SaveChangesAsync();

        return affected;
    }
    #endregion

    #region IDisposable
    private bool disposedValue;

    protected virtual void InternalDispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (this.context != null)
            {
                this.context.Dispose();
            }

            disposedValue = true;
        }
    }

    ~DataServiceBase()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        InternalDispose(disposing: false);
    }
    #endregion
}
