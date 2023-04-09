namespace PTTDigital.Email.Application.Models;

public class OAuthUserModel
{
    /// <summary>
    /// AccPolicys.Id
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Accounts.Email
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Applications.Id
    /// </summary>
    public string? AppId { get; set; }

    /// <summary>
    /// Applications.AppName
    /// </summary>
    public string? AppName { get; set; }

    /// <summary>
    /// Permissions.Rights API
    /// </summary>
    public string[]? Roles { get; set; }

    /// <summary>
    /// Permissions.Rights MENU
    /// </summary>
    public List<MenuPermission>? MenuPermissions { get; set; }
}