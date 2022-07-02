using Rtrw.Client.Wasm.Enums;

namespace Rtrw.Client.Wasm.Models
{
    public class Reaction
    {
        public string Id { get; set; } = $"reaction-{Guid.NewGuid().ToString("N")}";
        public Warga Reactor { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Emoji Emoji { get; set; }
    }
}
