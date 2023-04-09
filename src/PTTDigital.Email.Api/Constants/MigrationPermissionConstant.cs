namespace PTTDigital.Email.Api.Constants;

public sealed class MigrationPermissionConstant
{
    public IPermissionConstant GetPermissionConstant()
    {
        return new PermissionConstant();
    }
}
