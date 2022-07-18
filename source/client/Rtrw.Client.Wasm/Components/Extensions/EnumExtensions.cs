using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;

namespace Rtrw.Client.Wasm.Components.Extensions
{
    public static class EnumExtensions
    {
        private static readonly ConcurrentDictionary<string, string> DisplayNameCache = new ConcurrentDictionary<string, string>();

        public static string EnumToDescriptionString(this Enum value)
        {
            var attributes = (DescriptionAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString().ToLower();
        }

        public static string DisplayName(this Enum value)
        {
            var key = $"{value.GetType().FullName}.{value}";

            var displayName = DisplayNameCache.GetOrAdd(key, x =>
            {
                var name = (DescriptionAttribute[])value
                    .GetType()
                    .GetTypeInfo()
                    .GetField(value.ToString())
                    .GetCustomAttributes(typeof(DescriptionAttribute), false);

                return name.Length > 0 ? name[0].Description : value.ToString();
            });

            return displayName;
        }
    }
}
