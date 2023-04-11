using PTTDigital.Email.Common.ApplicationUser.User;
using PTTDigital.Email.Common.Configuration.AppSetting;
using PTTDigital.Email.Common.Configuration.AppSetting.API.AuthenticationConstant;

namespace PTTDigital.Email.Application.Api.Client.HttpDelegating;

public class OperationHandler : DelegatingHandler
{
    private readonly IApplicationUser applicationUser;
    private readonly IAppSetting appSetting;

    public OperationHandler(IApplicationUser applicationUser,
                            IAppSetting appSetting) 
    {
        this.applicationUser = applicationUser;
        this.appSetting = appSetting;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(applicationUser.SessionToken)) //JWT
        {
            request.Headers.Add(EmailConstant.HttpHeaderSessionToken, applicationUser.SessionToken);
        }

        if (!string.IsNullOrWhiteSpace(applicationUser.RemoteAddress))
        {
            request.Headers.Add(EmailConstant.HttpHeaderForwardedFor, applicationUser.RemoteAddress);
        }

        //if (!string.IsNullOrWhiteSpace(applicationUser.TraceId))
        //{
        //    request.Headers.Add(PpeOnlineConstant.HttpHeaderTraceId, applicationUser.TraceId);
        //}

        request.Headers.Add(EmailConstant.HttpHeaderTraceId, $"{Ulid.NewUlid()}");

        if (!string.IsNullOrWhiteSpace(appSetting.AppId))
        {
            request.Headers.Add(EmailConstant.HttpHeaderAppId, appSetting.AppId);
        }

        if (!string.IsNullOrWhiteSpace(appSetting.AppName))
        {
            request.Headers.Add(EmailConstant.HttpHeaderAppName, appSetting.AppName);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
