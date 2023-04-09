using System.ComponentModel;

namespace PTTDigital.Email.Common.Helper;

public static class EnumExtension
{
    public static T ToEnum<T>(this string value, T defaultValue) where T : new()
    {
        return GetEnum(value, defaultValue);
    }

    public static string ToDescription(this Enum item)
    {
        var memberInfo = item.GetType().GetMember(item.ToString()).FirstOrDefault();

        if (memberInfo == null) return string.Empty;

        var attribute = (DescriptionAttribute)memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
        return attribute?.Description ?? item.ToString();
    }

    private static T GetEnum<T>(string value, T defaultValue)
    {
        if (!typeof(T).IsEnum || (string.IsNullOrEmpty(value.Trim())))
            return defaultValue;

        try
        {
            return (T)Enum.Parse(typeof(T), value.Trim(), true);
        }
        catch
        {
            try
            {
                var type = typeof(T);
                var name = type.GetMembers().Where(c =>
                {
                    var attributes = c.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (attributes.Length > 0 && attributes[0] is DescriptionAttribute descAttr)
                    {
                        var description = descAttr.Description;
                        return string.Compare(description.Trim(), value.Trim(), true) == 0;
                    }

                    return false;
                }).Select(c => c.Name).FirstOrDefault();

                if (string.IsNullOrEmpty(name))
                    return defaultValue;
                else
                    return (T)Enum.Parse(typeof(T), name, true);
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
