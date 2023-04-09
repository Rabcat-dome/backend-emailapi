namespace PTTDigital.Email.Common.ApplicationUser.User;

public interface IClaimPayload
{
    /// <summary>
    /// Identifier for the Session. 
    /// </summary>
    string? Sid { get; }

    string? UserId { get; }

    string? Email { get; }

    string? AppId { get; }

    string? AppName { get; set; }

    string[] Roles { get; }

    string? ClientId { get; }
}
