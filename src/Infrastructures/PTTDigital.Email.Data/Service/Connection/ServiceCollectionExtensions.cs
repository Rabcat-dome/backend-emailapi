using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PTTDigital.Email.Common.Configuration.AppSetting;
using PTTDigital.Email.Data.Interfaces;

namespace PTTDigital.Email.Data.Service.Connection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEntityConfig<TContext>(this IServiceCollection services, IConfiguration configuration, string name)
        where TContext : DbContext
    {
        var useInMemory = configuration.GetValue<bool>(nameof(IAppSetting.UseInMemory));
        var configurationSection = configuration.GetSection(name);
        services.Configure<EntityConfig<TContext>>(configurationSection);
        services.AddSingleton<IEntityConfig<TContext>>(sp =>
        {
            var options = sp.GetService<IOptions<EntityConfig<TContext>>>();
            if (options == null)
            {
                return EntityConfig<TContext>.Instance;
            }

            var result = options.Value;
            result.UseInMemory = useInMemory;

            return result;
        });
        return services;
    }
}

