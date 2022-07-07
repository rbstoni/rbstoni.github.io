namespace Rtrw.Client.Wasm.Models
{
    public class Medium
    {
        public string Id { get; set; } = $"media-{Guid.NewGuid().ToString("N")}";
        // public Warga Uploader { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public byte[]? FileBytes { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
        public string? FileType { get; set; }
        public long? FileSize { get; set; }
    }
}

