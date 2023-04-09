using Newtonsoft.Json;

namespace PTTDigital.Email.Common.Helper;

public static class JsonHelper
{
    public static string SerializeObject(object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }

    public static T DeserializeObject<T>(object obj)
    {
        return DeserializeObject<T>(JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        }));
    }

    public static T DeserializeObject<T>(string jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            throw new ArgumentException("JsonHelper.DeserializeObject : Parameter cannot be null.");
        }

        T obj = JsonConvert.DeserializeObject<T>(jsonString);

        return obj;
    }
}
