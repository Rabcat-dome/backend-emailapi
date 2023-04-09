using Microsoft.EntityFrameworkCore;
using PTTDigital.Email.Data.Context;
using PTTDigital.Email.Data.Interfaces;

namespace PTTDigital.Email.Data.SqlServer.Context;

public sealed class EmailDataContext : EmailContextBase<EmailDataContext>
{
    private readonly IEntityConfig<EmailDataContext> _config;

    public EmailDataContext(DbContextOptions<EmailDataContext> options, IEntityConfig<EmailDataContext> config) : 
        base(options, config)
    {
        this._config = config;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {


        ApplyDatabaseOptions(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!_config.UseInMemory && !string.IsNullOrWhiteSpace(_config?.Schema))
            optionsBuilder.UseSqlServer(x => x.MigrationsHistoryTable("__EFMigrationsHistory", _config?.Schema));
    }
}
