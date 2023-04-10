using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PTTDigital.Email.Api.HealthCheck;

public class KeyVaultHealthCheck : IHealthCheck
{ 
    public KeyVaultHealthCheck()
    {

    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy("KeyVault up"));
    }
}
