namespace PTTDigital.Email.Api.HealthCheck;

public class HealthCheckPath : IHealthCheckPath
{
    private readonly string[] byPass;

    public HealthCheckPath(string[] byPass)
    {
        this.byPass = byPass;
    }

    public bool IsByPass(string path) 
    {
        if(path.StartsWith("/"))
           path = path.Replace("/", "");

        if (Array.Exists(byPass, x => x.Contains(path.ToLowerInvariant())))
            return true;
        return false;
    } 
}
