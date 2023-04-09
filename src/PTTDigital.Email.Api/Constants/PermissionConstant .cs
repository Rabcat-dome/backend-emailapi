namespace PTTDigital.Email.Api.Constants;

internal class PermissionConstant : IPermissionConstant
{
    internal const string ACCOUNTGROUP_VIEW = "Auth.AccountGroup.View";
    internal const string ACCOUNTGROUP_ADD = "Auth.AccountGroup.Add";
    internal const string ACCOUNTGROUP_EDIT = "Auth.AccountGroup.Edit";
    internal const string ACCOUNTGROUP_DELETE = "Auth.AccountGroup.Delete";
    internal const string ACCOUNTGROUP_LIST = "Auth.AccountGroup.List";
    internal const string ACCOUNTGROUP_PRINT = "Auth.AccountGroup.Print";

    internal const string ACCOUNT_VIEW = "Auth.Account.View";
    internal const string ACCOUNT_ADD = "Auth.Account.Add";
    internal const string ACCOUNT_EDIT = "Auth.Account.Edit";
    internal const string ACCOUNT_DELETE = "Auth.Account.Delete";
    internal const string ACCOUNT_LIST = "Auth.Account.List";
    internal const string ACCOUNT_PRINT = "Auth.Account.Print";

    internal const string ACCPOLICY_VIEW = "Auth.AccPolicy.View";
    internal const string ACCPOLICY_ADD = "Auth.AccPolicy.Add";
    internal const string ACCPOLICY_EDIT = "Auth.AccPolicyount.Edit";
    internal const string ACCPOLICY_DELETE = "Auth.AccPolicy.Delete";
    internal const string ACCPOLICY_LIST = "Auth.AccPolicy.List";
    internal const string ACCPOLICY_PRINT = "Auth.AccPolicy.Print";

    internal const string APPCOMP_VIEW = "Auth.AppComp.View";
    internal const string APPCOMP_ADD = "Auth.AppComp.Add";
    internal const string APPCOMP_EDIT = "Auth.AppComp.Edit";
    internal const string APPCOMP_DELETE = "Auth.AppComp.Delete";
    internal const string APPCOMP_LIST = "Auth.AppComp.List";
    internal const string APPCOMP_PRINT = "Auth.AppComp.Print";

    internal const string APPLICATION_VIEW = "Auth.Application.View";
    internal const string APPLICATION_ADD = "Auth.Application.Add";
    internal const string APPLICATION_EDIT = "Auth.Application.Edit";
    internal const string APPLICATION_DELETE = "Auth.Application.Delete";
    internal const string APPLICATION_LIST = "Auth.Application.List";
    internal const string APPLICATION_PRINT = "Auth.Application.Print";

    internal const string COMPANY_VIEW = "Auth.Company.View";
    internal const string COMPANY_ADD = "Auth.Company.Add";
    internal const string COMPANY_EDIT = "Auth.Company.Edit";
    internal const string COMPANY_DELETE = "Auth.Company.Delete";
    internal const string COMPANY_LIST = "Auth.Company.List";
    internal const string COMPANY_PRINT = "Auth.Company.Print";

    internal const string GROUP_VIEW = "Auth.Group.View";
    internal const string GROUP_ADD = "Auth.Group.Add";
    internal const string GROUP_EDIT = "Auth.Group.Edit";
    internal const string GROUP_DELETE = "Auth.Group.Delete";
    internal const string GROUP_LIST = "Auth.Group.List";
    internal const string GROUP_PRINT = "Auth.Group.Print";

    internal const string GROUPGROUPROLE_VIEW = "Auth.GroupGroupRole.View";
    internal const string GROUPGROUPROLE_ADD = "Auth.GroupGroupRole.Add";
    internal const string GROUPGROUPROLE_EDIT = "Auth.GroupGroupRole.Edit";
    internal const string GROUPGROUPROLE_DELETE = "Auth.GroupGroupRole.Delete";
    internal const string GROUPGROUPROLE_LIST = "Auth.GroupGroupRole.List";
    internal const string GROUPGROUPROLE_PRINT = "Auth.GroupGroupRole.Print";

    internal const string PERMISSION_VIEW = "Auth.Permission.View";
    internal const string PERMISSION_ADD = "Auth.Permission.Add";
    internal const string PERMISSION_EDIT = "Auth.Permission.Edit";
    internal const string PERMISSION_DELETE = "Auth.Permission.Delete";
    internal const string PERMISSION_LIST = "Auth.Permission.List";
    internal const string PERMISSION_PRINT = "Auth.Permission.Print";

    internal const string ROLE_VIEW = "Auth.Role.View";
    internal const string ROLE_ADD = "Auth.Role.Add";
    internal const string ROLE_EDIT = "Auth.Role.Edit";
    internal const string ROLE_DELETE = "Auth.Role.Delete";
    internal const string ROLE_LIST = "Auth.Role.List";
    internal const string ROLE_PRINT = "Auth.Role.Print";

    internal const string ROLEPERMISSION_VIEW = "Auth.RolePermission.View";
    internal const string ROLEPERMISSION_ADD = "Auth.RolePermission.Add";
    internal const string ROLEPERMISSION_EDIT = "Auth.RolePermission.Edit";

    public Dictionary<string, string?> GetRawConstantValue()
    {
        var result = typeof(PermissionConstant).GetRuntimeFields().ToDictionary(x => x.Name, x => x.GetRawConstantValue()?.ToString());
        return result;
    }
}
