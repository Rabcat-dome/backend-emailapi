using Microsoft.Extensions.Diagnostics.HealthChecks;
using PTTDigital.Email.Data.SqlServer.Context;

namespace PTTDigital.Email.Api.HealthCheck;

public class ContextHealthCheck : IHealthCheck
{
    public ContextHealthCheck(EmailDataContext AuthorizationDataContext)
    {
        this.AuthorizationDataContext = AuthorizationDataContext;
    }

    public EmailDataContext AuthorizationDataContext { get; }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (AuthorizationDataContext.Database.CanConnect())
        {
            return Task.FromResult(HealthCheckResult.Healthy("Database up"));
        }

        return Task.FromResult(HealthCheckResult.Unhealthy("Database down"));
    }
}
