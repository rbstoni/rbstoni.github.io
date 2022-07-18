namespace Rtrw.Client.Wasm.Extensions
{
    public static class TimeSpanExtensions
    {
        public static string TimeSpanToString(this DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;
            if (timeSpan.Days > 30)
                return $"{dateTime:dd/MM/yyyy}";
            if (timeSpan.Days > 1)
                return $"{timeSpan.Days} hari yang lalu";
            if (timeSpan.Hours > 1)
                return $"{timeSpan.Hours} jam yang lalu";
            if (timeSpan.Minutes < 5)
                return $"{dateTime: HH:mm}";

            return $"{dateTime:dd/MM/yyyy}";
        }
    }
}
