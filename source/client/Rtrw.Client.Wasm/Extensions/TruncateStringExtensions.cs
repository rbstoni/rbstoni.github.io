namespace Rtrw.Client.Wasm.Extensions
{
    public static class TruncateStringExtensions
    {
        public static string TruncateString(this string str, int maxChar)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > maxChar && maxChar > 0)
                return str.Substring(0, maxChar);
            return string.Empty;
        }
    }
}
