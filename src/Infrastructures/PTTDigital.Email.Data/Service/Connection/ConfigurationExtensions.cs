using System.Reflection;
using Microsoft.Extensions.Configuration;
using PTTDigital.Email.Common.KeyVault;

namespace PTTDigital.Email.Data.Service.Connection;

public static class ConfigurationExtensions
{
    public static string GetConnectionSettings<TConnectionSettings>(this IConfiguration configuration, IServiceProvider service, string name)
        where TConnectionSettings : ConnectionSettings
    {
        const string section = nameof(ConnectionSettings);
        return GetConnectionString<TConnectionSettings>(configuration, service, section, name);
    }

    public static string GetConnectionString<TConnectionSettings>(this IConfiguration configuration, IServiceProvider service, string section, string name)
        where TConnectionSettings : ConnectionSettings
    {
        var configurationSection = configuration.GetSection(section);
        var settings = GetConnectionSettings<TConnectionSettings>(configurationSection, service, section);
        if (settings == null)
        {
            return default;
        }

        var result = settings.FirstOrDefault(c => c.ConnectionName.Equals(name));
        return result?.GetConnectionString();
    }

    internal static IEnumerable<IConnectionSettings> GetConnectionSettings<TConnectionSettings>(IConfigurationSection configurationSection, IServiceProvider service, string name)
    {
        var propertyItems = GetPropertyItems(configurationSection, name);
        var type = typeof(TConnectionSettings);
        var props = type.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);

        var result = propertyItems.GroupBy(c => c.Key).Select(grp =>
        {
            var groupItems = grp.ToList();
            groupItems.Add(new PropertyItem
            {
                Key = grp.Key,
                PropName = nameof(ConnectionSettings.ConnectionName),
                Value = grp.Key
            });

            var obj = CreateObject(type, service, groupItems, props);

            return obj;
        }).OfType<IConnectionSettings>();

        return result;
    }

    private static IEnumerable<PropertyItem> GetPropertyItems(IConfigurationSection configurationSection, string name)
    {
        var regex = new System.Text.RegularExpressions.Regex(@$"^{name}:(?<key>\w+):(?<propname>\w+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        var items = configurationSection.AsEnumerable().ToArray();

        var result = items.Where(c => !string.IsNullOrEmpty(c.Value)).Select(item =>
        {
            var match = regex.Match(item.Key);
            if (match.Success)
            {
                var keytext = match.Groups["key"].Value;
                var nametext = match.Groups["propname"].Value;
                if (!string.IsNullOrEmpty(keytext))
                {
                    return new PropertyItem
                    {
                        Key = keytext,
                        PropName = nametext,
                        Value = item.Value,
                    };
                }
            }
            return default;
        }).Where(c => c != null);

        return result;
    }

    private static object CreateObject(Type type, IServiceProvider service, IEnumerable<PropertyItem> propertyItems, IEnumerable<PropertyInfo> propertyInfos)
    {
        var keyvaultService = service.GetService<IKeyVaultService>();
        var obj = Activator.CreateInstance(type, keyvaultService);

        foreach (var prop in propertyInfos)
        {
            var itemValue = propertyItems.FirstOrDefault(c => c.PropName.Equals(prop.Name))?.Value;
            switch (Type.GetTypeCode(prop.PropertyType))
            {
                case TypeCode.Int32:
                    {
                        if (int.TryParse(itemValue, out int valInt))
                        {
                            prop.SetValue(obj, valInt, null);
                        }
                    }
                    break;
                case TypeCode.String:
                    prop.SetValue(obj, itemValue, null);
                    break;
                case TypeCode.Boolean:
                    if (bool.TryParse(itemValue, out bool valBool))
                    {
                        prop.SetValue(obj, valBool, null);
                    }
                    break;
                case TypeCode.Object:
                    {
                        if (prop.PropertyType.Equals(typeof(int?)) && int.TryParse(itemValue, out int valInt))
                        {
                            prop.SetValue(obj, valInt, null);
                        }
                        else if (prop.PropertyType.Equals(typeof(decimal?)) && decimal.TryParse(itemValue, out decimal valDecimal))
                        {
                            prop.SetValue(obj, valDecimal, null);
                        }
                    }
                    break;
            }
        }

        return obj;
    }
}