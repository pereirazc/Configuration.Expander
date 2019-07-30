using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Microsoft.Extensions.Configuration.Expander
{
    public static class ConfigurationExtensions
    {

        private const string _EXPANDABLE_VALUE_REGEX_PATTERN = @"\$\{\s*\?(?<key>\S+)(\s+\?\?\s+(?<default>\S+)\s*)?}";


        public static T ResolveValue<T>(this IConfiguration configuration, string key)
        {
            Regex pattern = new Regex(_EXPANDABLE_VALUE_REGEX_PATTERN);

            var value = configuration.GetValue<string>(key);

            var match = pattern.Match(value);
            if (match.Success)
            {
                var innerKey = match.Groups["key"].Value;
                if (string.IsNullOrEmpty(innerKey))
                {
                    throw new InvalidOperationException($"Couldn't find any variable to expand in '{key}'");
                }
                var defaultValueStr = match.Groups["default"].Value;
                if (string.IsNullOrEmpty(defaultValueStr))
                {
                    return configuration.GetValue<T>(innerKey);
                }
                else
                {
                    var convertedDefaultValue = (T)ConvertValue(typeof(T), defaultValueStr);
                    return configuration.GetValue<T>(innerKey, convertedDefaultValue);
                }
            }
            else
            {
                return configuration.GetValue<T>(key);
            }
        }

        private static bool TryConvertValue(Type type, string value, out object result, out Exception error)
        {
            error = null;
            result = null;
            if (type == typeof(object))
            {
                result = value;
                return true;
            }

            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (string.IsNullOrEmpty(value))
                {
                    return true;
                }
                return TryConvertValue(Nullable.GetUnderlyingType(type), value, out result, out error);
            }

            var converter = TypeDescriptor.GetConverter(type);
            if (converter.CanConvertFrom(typeof(string)))
            {
                try
                {
                    result = converter.ConvertFromInvariantString(value);
                }
                catch (Exception ex)
                {
                    error = new InvalidOperationException(
                                    string.Format(CultureInfo.CurrentCulture, "Failed to convert configuration value to type '{0}'", type), ex);
                }
                return true;
            }
            return false;
        }

        private static object ConvertValue(Type type, string value)
        {
            object result;
            Exception error;
            TryConvertValue(type, value, out result, out error);
            if (error != null)
            {
                throw error;
            }
            return result;
        }
    }

}
