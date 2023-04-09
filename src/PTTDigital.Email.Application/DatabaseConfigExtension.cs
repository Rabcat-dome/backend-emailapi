using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PTTDigital.Email.Common.KeyVault;
using PTTDigital.Email.Data.Service.Connection;
using PTTDigital.Email.Data.SqlServer;
using PTTDigital.Email.Data.SqlServer.Context;

namespace PTTDigital.Email.Application;

public static class DatabaseConfigExtension
{
    private const string ConfigUseInMemory = "UseInMemory";
    private const string InMemoryDatabaseName = "InMemoryDatabase";

    public static void AddDatabaseConfiguration(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        builder.Services.AddScoped<IKeyVaultService, KeyVaultService>();
        builder.Services.AddDbContext<EmailDataContext>((service, options) =>
        {
            var useInMemory = configuration.GetValue<bool>(ConfigUseInMemory);

            if (useInMemory)
            {
                options.UseInMemoryDatabase(databaseName: InMemoryDatabaseName);
            }
            else
            {
                var connectionString = builder.Configuration.GetConnectionString<SqlConnectionSettings>(service, "ConnectionSettings", "DataConnection");
                options.UseSqlServer(connectionString);
            }
        });
    }
}
