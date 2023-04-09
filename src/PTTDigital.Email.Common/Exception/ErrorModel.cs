namespace PTTDigital.Email.Common.Exception;

public class ErrorModel
{
    [JsonProperty("error")]
    public string Error { get; set; }

    [JsonProperty("error_description")]
    public string Description { get; set; }
}
