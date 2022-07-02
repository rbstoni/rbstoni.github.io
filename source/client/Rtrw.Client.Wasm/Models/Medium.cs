namespace Rtrw.Client.Wasm.Models
{
    public class Medium
    {
        public string Id { get; set; } = $"media-{Guid.NewGuid().ToString("N")}";
        // public Warga Uploader { get; set; }
        public DateTime CreatedAt { get; set; }
        public string MediumUrl { get; set; }
        public string? Type { get; set; }
    }
}

