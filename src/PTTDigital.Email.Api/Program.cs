﻿using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using PTTDigital.Email.Api.Provider;
using PTTDigital.Email.Api.Provider.Dependency;
using PTTDigital.Email.Api.Provider.Swagger;
using PTTDigital.Email.Application;
using PTTDigital.Email.Application.Authorization;
using PTTDigital.Email.Application.Models.Token;
using PTTDigital.Email.Common.Configuration.AppSetting;
using PTTDigital.Email.Common.Configuration.AppSetting.API;
using PTTDigital.Email.Common.KeyVault;

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
builder.AddLogger();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerVersioning();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
