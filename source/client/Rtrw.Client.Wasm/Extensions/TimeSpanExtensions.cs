namespace Rtrw.Client.Wasm.Extensions
{
    public static class TimeSpanExtensions
    {
        public static string TimeSpanToString(this DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;
            if (timeSpan.Days < 1)
                return $"{dateTime:HH:mm}";
            if (timeSpan.Hours < 12)
                return $"{timeSpan.Hours} yang lalu";
            if (timeSpan.Minutes < 5)
                return "Baru saja";
            return dateTime.ToString("dd/MM/yyyy");
        }
    }
}
