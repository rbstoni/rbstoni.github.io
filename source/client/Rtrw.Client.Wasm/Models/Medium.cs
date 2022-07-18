namespace Rtrw.Client.Wasm.Models
{
    public class Medium
    {
        public string Id { get; set; } = $"media-{Guid.NewGuid().ToString("N")}";
        // public Warga Uploader { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.Now;
        //public byte[]? Bytes { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? ContentType { get; set; }
        public long? Size { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string? GpsLongitude { get; set; }
        public string? GpsLatitude { get; set; }
        public string? GpsLongitudeRef { get; set; }
        public string? GPSLatitudeRef { get; set; }
    }
}

