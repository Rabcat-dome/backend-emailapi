using PTTDigital.Email.Application.Models.Token;
using PTTDigital.Email.Common.ApplicationUser;
using PTTDigital.Email.Common.Configuration.AppSetting;
using PTTDigital.Email.Common.Configuration.AppSetting.API.AuthenticationConstant;
using PTTDigital.Email.Common.Helper;
using System.Net;

namespace PTTDigital.Email.Api.Middleware;

internal class CustomAuthorizeMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomAuthorizeMiddleware> _logger;

    public CustomAuthorizeMiddleware(RequestDelegate next, ILogger<CustomAuthorizeMiddleware> logger)
    {
        this._next = next;
        this._logger = logger;
    }


    public async Task InvokeAsync(HttpContext context,
                                  IAppSetting appSetting,
                                  IJwtManager jwtManager,
                                  //IApplicationUser appUser,
                                  IHttpClientFactory httpClientFactory)
    {
        //_logger.Log(LogLevel.Debug, LogEvents.Middleware, "Begin authorization", appUser);

        //var authorization = context.Request.Headers.Authorization.ToString();
        //if (string.IsNullOrWhiteSpace(authorization))
        //{
        //    _logger.Log(LogLevel.Debug, LogEvents.Middleware, "Request without authorization", appUser);

        //    await _next(context);
        //    return;
        //}

        //var authValue = PpeOnlineConstant.GetAuthValue(authorization);
        //if (!authValue.IsValid)
        //{
        //    _logger.Log(LogLevel.Debug, LogEvents.Middleware, "Request with invalid authorization", appUser);

        //    await _next(context);
        //    return;
        //}

        //if (!authValue.IsGuid)
        //{
        //    _logger.Log(LogLevel.Debug, LogEvents.Middleware, "Request with JWT authorization", appUser);

        //    context.Items.TryAdd(PpeOnlineConstant.HttpHeaderSessionToken, authValue.Value);
        //    await _next(context);
        //    return;
        //}

        SessionTokenModel model = new SessionTokenModel();
        ByteArrayContent? byteArrayContent = null;
        try
        {
            //_logger.Log(LogLevel.Debug, LogEvents.Middleware, "Request with access token", appUser);

            var path = context.Request.Path.ToString().GetApi();

            if (path.Equals(ApiClientType.ApiPpe.ToString().ToLowerInvariant()))
            {
                byteArrayContent = await context.GetContents();

                var reqMessage = await context.GetHttpRequestMessage(appSetting.OAuthSession!, HttpMethod.Get, appSetting.ApiClients![ApiClientType.ApiAuth.ToString()]!.ApiKey!);
                var resMessage = await httpClientFactory.CreateClient(ApiClientType.ApiAuth.ToString()).SendAsync(reqMessage);

                if (!resMessage.IsSuccessStatusCode)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Response.Headers.Add("Access-Control-Allow-Origin", appSetting.AllowOrigins);
                    return;
                }

                using var stream = await resMessage.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(stream);
                string jwt = await reader.ReadToEndAsync();
                var sessionToken = JsonHelper.DeserializeObject<SessionTokenModel>(jwt);
                model.SessionToken = sessionToken.SessionToken;
                model.IsSuccessStatusCode = !string.IsNullOrEmpty(model.SessionToken);
                model.StatusCode = HttpStatusCode.OK;
            }
        }
        catch (Exception ex)
        {
            //_logger.Log(LogLevel.Warning, LogEvents.Middleware, "Unhandled Validate Session Token.", appUser, ex);
        }

        if (model is not null && model.IsSuccessStatusCode)
        {
            var claimsPrincipal = jwtManager.GetClaimsPrincipal(model.SessionToken);
            var isExpired = jwtManager.IsExpiredDate(claimsPrincipal);
            if (!isExpired)
            {
                context.Items.TryAdd(nameof(SessionTokenModel), model);
                context.Items.TryAdd(EmailConstant.HttpHeaderSessionToken, model.SessionToken);
                context.User = claimsPrincipal;
                context.Request.Body = await byteArrayContent!.ReadAsStreamAsync();
            }
        }

        await _next(context);
    }
}