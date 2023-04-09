namespace PTTDigital.Email.Application.Models
{
    public class OAuthResultModel
    {
        public bool Success { get; set; }
        public string DisplayMessage { get; set; }
        public string InternalMessage { get; set; }
    }

    public class OAuthResultModel<T> : OAuthResultModel
    {
        public T Value { get; set; }
    }
}
