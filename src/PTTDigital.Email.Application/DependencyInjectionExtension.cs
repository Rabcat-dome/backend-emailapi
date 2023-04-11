using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PTTDigital.Email.Application.Services;
using PTTDigital.Email.Common.ApplicationUser;
using PTTDigital.Email.Common.EncryptDecrypt.Cryptography;
using PTTDigital.Email.Data.Service;
using PTTDigital.Email.Data.SqlServer.Context;

namespace PTTDigital.Email.Application;

public static class DependencyInjectionExtension
{
    public static void AddDependencyInjection(this WebApplicationBuilder builder)
    {
        #region DI service ==> repository
        builder.Services.AddScoped<IGenerator, Generator>();
        builder.Services.AddScoped<IEmailQueueService, EmailQueueService>();
        builder.Services.AddScoped<IEmailTriggerService, EmailTriggerService>();
        builder.Services.AddScoped<IEncryptDecryptHelper, EncryptDecryptHelper>();
        builder.Services.RemoveAll<IJwtManager>().TryAddScoped<IJwtManager, JwtManager>();

        builder.Services.AddScoped<IEmailDataService>(service =>
        {
            var context = service.GetService<EmailDataContext>()!;
            var generator = service.GetService<IGenerator>()!;
            var memoryCache = service.GetService<IMemoryCache>()!;

            return new EmailDataService<EmailDataContext>(context, generator, memoryCache);
        });
        #endregion
    }
}
