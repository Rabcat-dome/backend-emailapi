namespace PTTDigital.Email.Common.ApplicationUser.User;

public class ClaimPayload : IClaimPayload
{
    [Description(ClaimTypes.Sid)]
    public string? Sid { get; set; }

    [Description(ClaimTypes.NameIdentifier)]
    public string? UserId { get; set; }

    [Description(ClaimTypes.Email)]
    public string? Email { get; set; }

    public string? AppId { get; set; }

    public string? AppName { get; set; }

    [Description(ClaimTypes.Role)]
    public string[]? Roles { get; set; }

    [Description(ClaimTypes.System)]
    public string? ClientId { get; set; }

    public ClaimPayload()
    {

    }

    public ClaimPayload(ClaimsPrincipal claimsPrincipal)
    {
        InitializeValues(claimsPrincipal);
    }

    #region Private Methods

    private void InitializeValues(ClaimsPrincipal principal)
    {
        if (principal is null)
            return;

        var props = this.GetType().GetProperties().Where(c => c.CanRead && c.CanWrite);
        var group = principal.Claims.GroupBy(c => c.Type).ToList();

        foreach (var grp in group)
        {
            var key = grp.Key;
            var prop = props.FirstOrDefault(prop => GetPropertyInfo(prop, key));
            if (prop is null)
            {
                continue;
            }

            var propType = prop.PropertyType;
            var values = grp.Select(c => c.Value).ToArray();
            if (values != null && values.Any() && propType.Equals(typeof(string[])))
            {
                prop.SetValue(this, values);
                continue;
            }

            var value = values.FirstOrDefault();
            if (string.IsNullOrEmpty(value))
            {
                continue;
            }

            if (propType.Equals(typeof(string)))
            {
                prop.SetValue(this, value);
            }
            else if ((propType.Equals(typeof(int)) || propType.Equals(typeof(int?))) && int.TryParse(value, out int valint))
            {
                prop.SetValue(this, valint);
            }
        }
    }

    private static bool GetPropertyInfo(PropertyInfo propInfo, string key)
    {
        if (propInfo.Name.Equals(key, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
        else
        {
            var attribute = (DescriptionAttribute)propInfo.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
            if (!string.IsNullOrEmpty(attribute?.Description) && attribute.Description.Equals(key))
            {
                return true;
            }
        }

        return false;
    }

    #endregion
}
