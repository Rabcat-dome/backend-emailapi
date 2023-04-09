namespace PTTDigital.Email.Application.Models;

public class TokenIdentity
{
    private TokenModel tokenModel = null;

    public string TokenKey { get; set; }
    public int SessionTokenExpireInMinutes { get; set; }

    public string ClientId { get; set; }
    public string ClientHash { get; set; }

    public OAuthUserModel UserModel { get; set; }
    public DateTime IssueDate { get; set; }

    public TokenModel TokenModel
    {
        get
        {
            if (tokenModel is null || IssueDate == DateTime.MinValue)
            {
                return tokenModel;
            }

            var time = DateTime.UtcNow.ToLocalTime() - IssueDate;
            var seconds = (int)time.TotalSeconds;

            var result = tokenModel.Clone();
            result.AccessTokenExpiresIn -= seconds;
            result.RefreshTokenExpiresIn -= seconds;

            return result;
        }
        set => tokenModel = value;
    }

    internal ClaimPayload GetClaimPayload(string traceId)
    {
        var sid = traceId?.Split(':')?.FirstOrDefault()?.Trim();
        return new ClaimPayload
        {
            Sid = sid,
            UserId = UserModel?.UserId!,
            Email = UserModel?.Email!,
            AppId = UserModel?.AppId!,
            AppName = UserModel?.AppName,
            Roles = UserModel?.Roles!,
            ClientId = ClientId
        };
    }
}
