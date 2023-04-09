using System.Net;
using Newtonsoft.Json;

namespace PTTDigital.Email.Application.Models.Token;

public class SessionTokenModel
{
    [JsonProperty("session_token")]
    public string SessionToken { get; set; }

    [JsonIgnore]
    public bool IsSuccessStatusCode { get; set; }

    [JsonIgnore]
    public HttpStatusCode StatusCode { get; set; }
}
