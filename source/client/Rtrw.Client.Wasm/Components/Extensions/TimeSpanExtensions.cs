namespace Rtrw.Client.Wasm.Components.Extensions
{
    public static class TimeSpanExtensions
    {
        public static string TimeSpanToString(this DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;
            if (timeSpan.Days < 1)
            {
                return $"{dateTime.ToString("HH:mm")}";
            }

            return dateTime.ToString("dd/MM/yyyy");
        }
    }
}
