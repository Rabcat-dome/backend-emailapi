namespace PTTDigital.Email.Common.Configuration.AppSetting;    

[Flags]
public enum ApiClientType
{
    None = 0,
    ApiGraph = 1,
    ApiAuth = 2,
    ApiEmp = 4,
    ApiInv = 8,
    ApiPpe = 16,

    All = ApiGraph | ApiAuth | ApiEmp | ApiInv | ApiPpe
}
