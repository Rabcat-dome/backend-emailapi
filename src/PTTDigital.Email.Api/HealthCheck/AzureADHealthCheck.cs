using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PTTDigital.Email.Api.HealthCheck;

public class AzureADHealthCheck : IHealthCheck
{ 
    public AzureADHealthCheck()
    {

    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy("AzureAD up"));
    }
}
