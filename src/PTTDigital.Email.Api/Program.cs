var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddEnvironmentVariables(prefix: "PTT_PPE_EMAIL_API_");
builder.Configuration.AddJsonFile("serilog.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"serilog.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
