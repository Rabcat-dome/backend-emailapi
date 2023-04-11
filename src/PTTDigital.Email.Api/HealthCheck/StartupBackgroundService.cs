using Hangfire;
using PTTDigital.Email.Application.Services;
using PTTDigital.Email.Common.Configuration.AppSetting;

namespace PTTDigital.Email.Api.HealthCheck;

public class StartupBackgroundService : BackgroundService
{
    private readonly StartupHealthCheck _healthCheck;
    private readonly IServiceProvider _serviceProvider;

    public StartupBackgroundService(StartupHealthCheck healthCheck, IServiceProvider serviceProvider)
    {
        _healthCheck = healthCheck;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var emailQueueService = scope.ServiceProvider.GetRequiredService<IEmailQueueService>();
            RecurringJob.AddOrUpdate("SendSMTPJob", () => emailQueueService.TriggerMail(), "*/2 * * * *"); //ทุก 2 นาที
            RecurringJob.AddOrUpdate("ArchiveMail", () => emailQueueService.ArchiveMail(), "0 1 * * *"); //ทุก 01.00น. ทุกวัน
            RecurringJob.AddOrUpdate("NotificationTicket", () => Console.WriteLine("-- Recurring everyday at 08:00 (15 Days Cases trigger PPE API or use schedule Type instead)"), "* 8 * * *");
        }
        _healthCheck.StartupCompleted = true;
        return Task.CompletedTask;
    }
}
