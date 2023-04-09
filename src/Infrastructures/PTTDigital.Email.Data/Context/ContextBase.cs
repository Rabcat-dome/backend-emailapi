using Microsoft.EntityFrameworkCore;

namespace PTTDigital.Email.Data.Context;

public abstract class DbContextBase<TDbContext> : DbContext where TDbContext : DbContext
{
    protected string? defaultSchema;
    protected string? defaultCollation;

    public virtual string? DefaultSchema => defaultSchema;
    public virtual string? DefaultCollation => defaultCollation;

    public DbContextBase()
    {
    }

    public DbContextBase(DbContextOptions<TDbContext> options)
    : base(options)
    {
    }

    protected virtual void ApplyDatabaseOptions(ModelBuilder modelBuilder)
    {
        if (!string.IsNullOrWhiteSpace(defaultSchema))
        {
            modelBuilder.HasDefaultSchema(defaultSchema);
        }

        if (!string.IsNullOrWhiteSpace(defaultCollation))
        {
            modelBuilder.UseCollation(defaultCollation);
        }
    }
}
