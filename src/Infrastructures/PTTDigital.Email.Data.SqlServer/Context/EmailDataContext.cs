using Microsoft.EntityFrameworkCore;
using PTTDigital.Email.Data.Context;
using PTTDigital.Email.Data.Interfaces;

namespace PTTDigital.Email.Data.SqlServer.Context;

public sealed class EmailDataContext : EmailContextBase<EmailDataContext>
{
    private readonly IEntityConfig<EmailDataContext> config;

    public EmailDataContext(DbContextOptions<EmailDataContext> options, IEntityConfig<EmailDataContext> config) : 
        base(options, config)
    {
        this.config = config;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {


        ApplyDatabaseOptions(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!config.UseInMemory && !string.IsNullOrWhiteSpace(config?.Schema))
            optionsBuilder.UseSqlServer(x => x.MigrationsHistoryTable("__EFMigrationsHistory", config?.Schema));
    }
}
