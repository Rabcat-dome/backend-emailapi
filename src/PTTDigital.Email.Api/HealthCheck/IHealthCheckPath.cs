namespace PTTDigital.Email.Api.HealthCheck;

public interface IHealthCheckPath
{
    bool IsByPass(string path);
}
