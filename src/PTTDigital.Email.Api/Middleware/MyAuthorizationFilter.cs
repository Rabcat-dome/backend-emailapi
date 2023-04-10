using Hangfire.Dashboard;

namespace PTTDigital.Email.Api.Middleware
{
    public class MyAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context) => true;
    }
}
