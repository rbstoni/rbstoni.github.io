using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Models
{
    public class MessageHistory
    {
        public string Id { get; set; } = $"message-history-{Guid.NewGuid().ToString("N")}";
        public List<Message> Messages { get; set; } = new();
    }
}
