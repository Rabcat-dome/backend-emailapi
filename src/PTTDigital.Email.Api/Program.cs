using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using PTTDigital.Email.Api.Provider;
using PTTDigital.Email.Api.Provider.Dependency;
using PTTDigital.Email.Api.Provider.Swagger;
using PTTDigital.Email.Application;
using PTTDigital.Email.Common.Configuration.AppSetting;
using PTTDigital.Email.Common.Configuration.AppSetting.API;
using PTTDigital.Email.Common.KeyVault;
using PTTDigital.Email.Data.SqlServer.Context;
using PTTDigital.Email.Data.Service.Connection;
using PTTDigital.Authentication.Api.HealthCheck;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using Microsoft.AspNetCore.ResponseCompression;
using PTTDigital.Email.Api.Middleware;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Mvc.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddEnvironmentVariables(prefix: "PTT_PPE_EMAIL_API_");
builder.Configuration.AddJsonFile("serilog.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"serilog.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

builder.Services.Configure<KeyVaultSettings>(builder.Configuration.GetSection("KeyVaultSettings"));
builder.Services.AddSingleton<IKeyVaultSettings>(service => service.GetService<IOptions<KeyVaultSettings>>()?.Value!);
builder.Services.AddScoped<IKeyVaultService, KeyVaultService>();

builder.Services.Configure<AppSetting>(builder.Configuration);
builder.Services.AddSingleton<IAppSetting>(service => service.GetService<IOptions<AppSetting>>()?.Value!);

builder.Services.AddSingleton<IAuthorizationConfig>(service => service.GetService<IOptions<AppSetting>>()?.Value.Authorization!);
//builder.Services.AddScoped<ISessionToken>(service => service.GetService<IApiAuthorizeServer>()!);
//builder.Services.AddScoped<IUserPermissionService, MockUserPermissionService>();  //ต้องใช้หรือไม่
builder.Services.AddHttpClientFactory();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true).AddNewtonsoftJson(options =>
{
    // Reseponse data for case (Pascal)
    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
    options.SerializerSettings.Formatting = Formatting.None;
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenVersioning();
builder.Services.AddSwaggerGenNewtonsoftSupport();
builder.Services.AddJwtTokenConfiguration(builder.Configuration);
builder.AddDependencyInjection();
builder.AddDatabaseConfiguration(builder.Configuration);
//builder.Services.AddAuthenticationService(builder.Configuration);
//builder.Services.AddRedisClientsManager(builder.Configuration);
builder.Services.AddEntityConfig<EmailDataContext>(builder.Configuration, "Email");
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddCors();

builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(@"/entity/protection/"));
builder.Services.AddCustomHealthChecks(builder.Configuration);

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.SmallestSize;
});

builder.AddLogger();

//hangfile
builder.Services.AddHangfire(config =>
    //หาน้องมาปรับ HardCode ตรงนี้ให้หน่อยครับ
    config.UseSqlServerStorage("Server=pttgc-poc.database.windows.net,1433;Database=ppe;User Id=ppe;Password=8yGH@#4Ut2o0;"));
builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerVersioning();
}

//hangfile
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DashboardTitle = "My PPE",
    Authorization = new[]
    {
        new HangfireCustomBasicAuthenticationFilter{
            User = "Admin",
            Pass = "Password"
        }
    }
});
//

app.UseMiddleware<TimestampMiddleware>();
app.UseMiddleware<DecompressionMiddleware>();
app.UseMiddleware<CustomAuthorizeMiddleware>();
app.UseMiddleware<ApiRouterMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCustomHealthCheck();
app.UseResponseCompression();
app.MapControllers();

app.Run();
