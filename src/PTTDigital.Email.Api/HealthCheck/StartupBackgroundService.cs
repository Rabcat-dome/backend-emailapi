using Hangfire;
using PTTDigital.Email.Application.Services;

namespace PTTDigital.Email.Api.HealthCheck;

public class StartupBackgroundService : BackgroundService
{
    private readonly StartupHealthCheck _healthCheck;
    private readonly IEmailQueueService _emailQueueService;

    public StartupBackgroundService(StartupHealthCheck healthCheck,IEmailQueueService emailQueueService)
    {
        _healthCheck = healthCheck;
        _emailQueueService = emailQueueService;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        RecurringJob.AddOrUpdate("SendSMTPJob", () => _emailQueueService.TriggerMail().Wait(stoppingToken), "*/2 * * * *"); //ทุก 2 นาที
        RecurringJob.AddOrUpdate("ArchiveMail", () => _emailQueueService.ArchiveMail().Wait(stoppingToken), "0 1 * * *"); //ทุก 01.00น. ทุกวัน
        RecurringJob.AddOrUpdate("NotificationTicket", () => Console.WriteLine("-- Recurring everyday at 08:00 (15 Days Cases trigger PPE API or use schedule Type instead)"), "* 8 * * *");

        _healthCheck.StartupCompleted = true;
        return Task.CompletedTask;
    }
}
