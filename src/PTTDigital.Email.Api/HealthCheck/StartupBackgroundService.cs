using Hangfire;

namespace PTTDigital.Email.Api.HealthCheck;

public class StartupBackgroundService : BackgroundService
{
    private readonly StartupHealthCheck _healthCheck;

    public StartupBackgroundService(StartupHealthCheck healthCheck)
        => _healthCheck = healthCheck;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        RecurringJob.AddOrUpdate("SendSMTPJob", () => Console.WriteLine("-- Recurring every 2 minutes OC 1--"), "*/2 * * * *");
        RecurringJob.AddOrUpdate("ArchiveMail", () => Console.WriteLine("-- Recurring everyday at 01:00 OC 2--"), "0 1 * * *");
        RecurringJob.AddOrUpdate("NotificationTicket", () => Console.WriteLine("-- Recurring everyday at 08:00 (15 Days Cases trigger PPE API or use schedule Type instead)"), "* 8 * * *");

        _healthCheck.StartupCompleted = true;
    }
}
