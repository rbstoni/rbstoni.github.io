using System.ComponentModel;

namespace Rtrw.Client.Wasm.Components.Extensions
{
    public static class EnumExtensions
    {
        public static string EnumToDescriptionString(this Enum value)
        {
            var attributes = (DescriptionAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString().ToLower();
        }
    }
}
