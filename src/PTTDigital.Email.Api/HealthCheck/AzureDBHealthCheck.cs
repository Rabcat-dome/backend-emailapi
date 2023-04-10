using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PTTDigital.Email.Api.HealthCheck;

public class AzureDBHealthCheck : IHealthCheck
{ 
    public AzureDBHealthCheck()
    {

    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy("AzureDB up"));
    }
}
