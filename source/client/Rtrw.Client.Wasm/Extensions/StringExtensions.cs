namespace Rtrw.Client.Wasm.Extensions
{
    public static class StringExtensions
    {
        public static string TruncateString(this string text, int maxChar)
        {
            const string suffix = "...";
            int truncatedFinalLength = maxChar - suffix.Length;
            if (maxChar <= 0) return text;
            if (truncatedFinalLength <= 0) return text;
            if (text == null || text.Length <= maxChar) return text;
            string truncatedString = text.Substring(0, maxChar);
            truncatedString = truncatedString.TrimEnd();
            truncatedString += suffix;

            return truncatedString;
        }

        public static double? StringToDouble(this string str)
        {
            if (!string.IsNullOrEmpty(str))
                return Convert.ToDouble(str);

            return null!;
        }
    }
}
