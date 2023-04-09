using PTTDigital.Email.Common.Redis;

namespace PTTDigital.Email.Application.Repositories.Token.TokenKeyPair
{

    internal record TokenKeyPair : ITokenKeyPair
    {
        public StoreAction Action { get; }
        public string Key { get; }
        public object Value { get; }
        public double ExpirationInMinutes { get; }

        public TokenKeyPair(StoreAction action, string key)
        {
            Action = action;
            Key = key;
        }

        internal TokenKeyPair(StoreAction action, string key, object value, double expirationInMinutes)
        {
            Action = action;
            Key = key;
            Value = value;
            ExpirationInMinutes = expirationInMinutes;
        }

        internal IRedisModel GetRedisModel()
        {
            return Action switch
            {
                StoreAction.Add => new RedisSetModel
                {
                    Key = Key,
                    ExpirationInMinutes = ExpirationInMinutes,
                    Value = new List<dynamic>(new[] { Value })
                },

                StoreAction.Delete => new RedisDeleteModel
                {
                    Key = Key,
                },

                _ => default
            };
        }
    }
}
