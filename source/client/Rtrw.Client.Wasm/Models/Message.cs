using System;
using System.Linq;

namespace Rtrw.Client.Wasm.Models
{
    public class Message
    {
        public string Id { get; set; } = $"message-{Guid.NewGuid().ToString("N")}";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? Text { get; set; }
        public bool Read { get; set; }
        public Warga Sender { get; set; }
    }
}
