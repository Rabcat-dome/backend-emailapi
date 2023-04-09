using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PTTDigital.Email.Application.Utility;

namespace PTTDigital.Email.Application.Models;

/// <remarks/>
public class AuthorizationModel
{
    /// <remarks/>
    [JsonProperty("grant_type")]
    [FromForm(Name = "grant_type")]
    public string GrantType { get; set; }

    /// <remarks/>
    [JsonProperty("email")]
    [FromForm(Name = "email")]
    public string Email { get; set; }

    /// <remarks/>
    //[JsonProperty("password")]
    //[FromForm(Name = "password")]
    //public string Password { get; set; }

    /// <remarks/>
    [JsonProperty("refresh_token")]
    [FromForm(Name = "refresh_token")]
    public string RefreshToken { get; set; }

    /// <remarks/>
    //[JsonProperty("client_id")]
    //[FromForm(Name = "client_id")]
    internal string ClientId => "PTTDigital.Authentication.Api";

    /// <remarks/>
    //[JsonProperty("client_secret")]
    //[FromForm(Name = "client_secret")]
    internal string ClientSecret => "RGV2X1Rlc3Q=";

    internal GrantTypeValue GrantTypeValue
    {
        get
        {
            var hasValue = Enum.TryParse(GrantType, true, out GrantTypeValue clientType);
            if (hasValue)
            {
                return clientType;
            }

            return GrantTypeValue.None;
        }
    }

    internal string GetHash()
    {
        var value = $"{Email}{ClientId}{ClientSecret}";
        var result = CryptorEngineHelper.GetHash(value);
        return result;
    }

    internal string GetClientHash()
    {
        var value = $"{ClientId}{ClientSecret}";
        var result = CryptorEngineHelper.GetHash(value);
        return result;
    }
}