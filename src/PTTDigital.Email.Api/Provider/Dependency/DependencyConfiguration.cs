using Microsoft.AspNetCore.Authentication.JwtBearer;
using PTTDigital.Email.Common.ApplicationUser;

namespace PTTDigital.Email.Api.Provider.Dependency;

/// <remarks />
public static class DependencyConfiguration
{
    /// <remarks />
    public static IServiceCollection AddJwtTokenConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = false;
            options.TokenValidationParameters = JwtManager.TokenValidationParameters();
        });

        return services;
    }
}
