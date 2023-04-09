using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PTTDigital.Email.Application.Models.Token
{
    [DataContract]
    public class TokenModel
    {
        [JsonProperty("access_token")]
        [DataMember(Name = "access_token")]
        public string? AccessToken { get; set; }

        [JsonProperty("expires_in")]
        [DataMember(Name = "expires_in")]
        public int AccessTokenExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        [DataMember(Name = "refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonProperty("refresh_token_expires_in")]
        [DataMember(Name = "refresh_token_expires_in")]
        public int RefreshTokenExpiresIn { get; set; }

        [JsonProperty("token_type")]
        [DataMember(Name = "token_type")]
        public string? TokenType { get; set; }

        [JsonIgnore]
        public DateTime AccessTokenExpiry { get; set; }

        [JsonIgnore]
        public DateTime RefreshTokenExpiry { get; set; }

        public TokenModel Clone()
        {
            return (TokenModel)MemberwiseClone();
        }
    }
}