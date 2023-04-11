using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PTTDigital.Email.Application.Services;
using PTTDigital.Email.Common.EncryptDecrypt.Cryptography;
using PTTDigital.Email.Data.Service;

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

        #endregion
    }
}
