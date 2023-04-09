using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using PTTDigital.Email.Application.Repositories;
using PTTDigital.Email.Common.EncryptDecrypt.Cryptography;

namespace PTTDigital.Email.Application;

public static class DependencyInjectionExtension
{
    public static void AddDependencyInjection(this WebApplicationBuilder builder)
    {
        #region DI service ==> repository
        builder.Services.AddScoped<IGenerator, Generator>();
        //builder.Services.AddScoped<IAccountService, AccountService>();

        builder.Services.AddScoped<IEncryptDecryptHelper, EncryptDecryptHelper>();

        #endregion
    }
}
