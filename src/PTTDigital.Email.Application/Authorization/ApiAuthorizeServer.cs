using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using PTTDigital.Email.Application.Models;
using PTTDigital.Email.Application.Authorization;
using PTTDigital.Email.Application.Models.Token;
using PTTDigital.Email.Application.Services.Token;
using PTTDigital.Email.Common.ApplicationUser;
using PTTDigital.Email.Common.Configuration.AppSetting.API.AuthenticationConstant;
using PTTDigital.Email.Common.Configuration.AppSetting.API.Client;
using PTTDigital.Email.Common.Exception;
using PTTDigital.Email.Common.Helper.LogExtension;

namespace PTTDigital.Email.Application.Authorization;

public class ApiAuthorizeServer : IApiAuthorizeServer
{
    private readonly IOAuthUserService authUserService;
    private readonly ITokenStore tokenStore;
    private readonly IJwtManager jwtManager;
    private readonly IClientConfigs clientConfigs;
    private readonly ILogger logger;

    public ApiAuthorizeServer(IOAuthUserService authUserService
        , ITokenStore tokenStore
        , IJwtManager jwtManager
        , ILoggerHelper loggerHelper
        , IClientConfigs clientConfigs)
    {
        this.authUserService = authUserService;
        this.tokenStore = tokenStore;
        this.jwtManager = jwtManager;
        this.clientConfigs = clientConfigs;

        logger = loggerHelper.CreateLogger(GetType());
    }

    public async Task<TokenModel> GrantAuthenticationAsync(AuthorizationModel model)
    {
        await Task.Yield();

        var validateResult = ValidateAuthorizationModel(model);
        if (!validateResult.Success)
        {
            throw new AuthorizationException(new ErrorModel
            {
                Error = AuthenticationConstant.InvalidClientError,
                Description = validateResult.DisplayMessage
            });
        }

        logger.Log(LogLevel.Debug, LogEvents.InternalService, model);

        return model.GrantTypeValue switch
        {
            GrantTypeValue.Password => await GrantPassword(model, validateResult.Value),
            GrantTypeValue.Refresh_Token => await GrantRefreshToken(model),
            _ => throw new NotSupportedException("grant type not supported")
        };
    }

    public async Task<SessionTokenModel> GetSessionTokenAsync(string accessToken)
    {
        var accessKey = $"{AuthenticationConstant.AccessKey}{accessToken}";
        var tokenId = await tokenStore.SearchAsync<TokenIdentity>($"{AuthenticationConstant.TokenKey}*:{accessKey}");

        if (tokenId is null)
        {
            var ex = new AuthorizationException(new ErrorModel
            {
                Error = AuthenticationConstant.InvalidClientError,
                Description = "Invalid access token"
            });

            logger.Log(LogLevel.Debug, LogEvents.InternalService, accessToken, ex);
            throw ex;
        }

        var traceId = ""; // applicationUser.TraceIdentifier;
        var claimPayload = tokenId.GetClaimPayload(traceId);
        var sessionToken = jwtManager.GenerateToken(claimPayload, tokenId.SessionTokenExpireInMinutes);

        return new SessionTokenModel
        {
            SessionToken = sessionToken,
            IsSuccessStatusCode = !string.IsNullOrEmpty(sessionToken),
            StatusCode = HttpStatusCode.OK,
        };
    }

    public async Task RevokeAuthenticationAsync(string accessToken)
    {
        var accessKey = $"{AuthenticationConstant.AccessKey}{accessToken}";
        var tokenId = await tokenStore.SearchAsync<TokenIdentity>($"{AuthenticationConstant.TokenKey}*:{accessKey}");

        if (tokenId is not null)
        {
            var tokenKey = tokenId.TokenKey;

            await tokenStore.SaveAsync(new[]
            {
                new TokenKeyPair(StoreAction.Delete, $"{tokenKey}:*")
            });
        }
    }

    private async Task<TokenModel> GrantPassword(AuthorizationModel model, OAuthApiClientModel client)
    {
        var hash = model.GetHash();
        var tokenKey = $"{AuthenticationConstant.TokenKey}{hash}";

        /* Use exists Token */
        var tokenId = await tokenStore.SearchAsync<TokenIdentity>($"{tokenKey}:{AuthenticationConstant.AccessKey}*");
        if (tokenId is not null)
        {
            return tokenId.TokenModel;
        }

        var resultModel = authUserService.VerifyAccount(model.Email);
        if (!resultModel.Success)
        {
            var ex = new AuthorizationException(new ErrorModel
            {
                Error = AuthenticationConstant.InvalidGrantError + "GrantResourceOwnerCredentials  result not Success",
                Description = resultModel.DisplayMessage
            });

            logger.Log(LogLevel.Debug, LogEvents.InternalService, model, ex);
            throw ex;
        }

        var clientHash = model.GetClientHash();
        var result = await CreateToken(tokenKey, clientHash, client, resultModel.Value);
        return result;
    }

    private async Task<TokenModel> GrantRefreshToken(AuthorizationModel model)
    {
        var refreshKey = $"{AuthenticationConstant.RefreshKey}{model.RefreshToken}";
        var tokenId = await tokenStore.SearchAsync<TokenIdentity>($"{AuthenticationConstant.TokenKey}*:{refreshKey}");

        if (tokenId is null)
        {
            throw new AuthorizationException(new ErrorModel
            {
                Error = AuthenticationConstant.InvalidClientError,
                Description = "Invalid token identity"
            });
        }

        if (!model.ClientId.Equals(tokenId.ClientId))
        {
            var ex = new AuthorizationException(new ErrorModel
            {
                Error = AuthenticationConstant.InvalidClientError,
                Description = "Invalid client identity"
            });

            logger.Log(LogLevel.Debug, LogEvents.InternalService, model, ex);
            throw ex;
        }

        var clientHash = model.GetClientHash();
        if (!clientHash.Equals(tokenId.ClientHash))
        {
            var ex = new AuthorizationException(new ErrorModel
            {
                Error = AuthenticationConstant.InvalidClientError,
                Description = "Invalid client credential"
            });

            logger.Log(LogLevel.Debug, LogEvents.InternalService, model, ex);
            throw ex;
        }

        var client = clientConfigs.GetClientId(tokenId.ClientId);
        var result = await CreateToken(tokenId.TokenKey, clientHash, client, tokenId.UserModel);
        return result;
    }

    private OAuthResultModel<OAuthApiClientModel> ValidateAuthorizationModel(AuthorizationModel model)
    {
        if (string.IsNullOrWhiteSpace(model?.ClientId))
        {
            return PTTDigital.Authentication.Application.Utility.ResultMessage<OAuthApiClientModel>.Error(Resources.ClientIdIsNullOrWhiteSpace);
        }

        if (string.IsNullOrWhiteSpace(model?.ClientSecret))
        {
            return PTTDigital.Authentication.Application.Utility.ResultMessage<OAuthApiClientModel>.Error(Resources.ClientSecretIsNullOrWhiteSpace);
        }

        var client = clientConfigs.GetClientId(model.ClientId);

        if (client == null)
        {
            return PTTDigital.Authentication.Application.Utility.ResultMessage<OAuthApiClientModel>.Error(Resources.InvalidClientId);
        }

        if (!client.ClientSecret.Equals(model.ClientSecret))
        {
            return PTTDigital.Authentication.Application.Utility.ResultMessage<OAuthApiClientModel>.Error(Resources.InvalidClientSecret);
        }

        return PTTDigital.Authentication.Application.Utility.ResultMessage<OAuthApiClientModel>.Success(client);
    }

    private async Task<TokenModel> CreateToken(string tokenKey, string clientHash, OAuthApiClientModel client, OAuthUserModel user)
    {
        ArgumentNullException.ThrowIfNull(tokenKey);
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(user);

        var accessToken = Guid.NewGuid().ToString();
        var refreshToken = Guid.NewGuid().ToString();

        var accessKey = $"{tokenKey}:{AuthenticationConstant.AccessKey}{accessToken}";
        var refreshKey = $"{tokenKey}:{AuthenticationConstant.RefreshKey}{refreshToken}";

        var tokenModel = new TokenModel
        {
            AccessToken = accessToken,
            AccessTokenExpiresIn = client.AccessTokenExpireInMinutes * 60,
            RefreshToken = refreshToken,
            RefreshTokenExpiresIn = client.RefreshTokenLifeTimeInMinutes * 60,
            TokenType = JwtBearerDefaults.AuthenticationScheme,
        };

        var tokenId = new TokenIdentity
        {
            TokenKey = tokenKey,
            SessionTokenExpireInMinutes = client.SessionTokenExpireInHour * 60,

            ClientId = client.ClientId,
            ClientHash = clientHash,

            UserModel = user,
            IssueDate = DateTime.UtcNow.ToLocalTime(),

            TokenModel = tokenModel,
        };

        await tokenStore.SaveAsync(new[]
        {
            new TokenKeyPair(StoreAction.Delete, $"{tokenKey}:*"),
            new TokenKeyPair(StoreAction.Add, accessKey, tokenId, client.AccessTokenExpireInMinutes),
            new TokenKeyPair(StoreAction.Add, refreshKey, tokenId, client.RefreshTokenLifeTimeInMinutes),
        });

        LoggerExtensions.Log(logger, (LogLevel)LogLevel.Debug, (string?)LogEvents.InternalService, new
        {
            AccessKey = accessKey,
            TokenKey = tokenKey,
            RefreshKey = refreshKey,
            TokenId = tokenId,
        });

        return tokenModel;
    }
}
