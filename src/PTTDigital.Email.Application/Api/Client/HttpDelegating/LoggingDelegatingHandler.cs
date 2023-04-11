using System.Diagnostics;
using Microsoft.Extensions.Logging;
using PTTDigital.Email.Common.ApplicationUser.User;

namespace PTTDigital.Email.Application.Api.Client.HttpDelegating;

public sealed class LoggingDelegatingHandler : DelegatingHandler
{
    private readonly IApplicationUser applicationUser;
    private readonly ILogger<HttpClient> logger;

    public LoggingDelegatingHandler(IApplicationUser applicationUser, ILogger<HttpClient> logger)
    {
        this.applicationUser = applicationUser;
        this.logger = logger;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Exception exception = null;
        HttpResponseMessage response = null;
        var sw = Stopwatch.StartNew();
        try
        {
            var task = base.SendAsync(request, cancellationToken);
            response = task.Result;
            return task;
        }
        catch (Exception ex)
        {
            exception = ex;
            return Task.FromException<HttpResponseMessage>(ex);
        }
        //finally
        //{
        //    LogHttpClient(request.RequestUri, request, response, sw.Elapsed, exception);
        //}
    }   
}
