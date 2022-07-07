using Rtrw.Client.Wasm.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Models
{
    public class Contact
    {
        public string Id { get; set; } = $"contact-{Guid.NewGuid().ToString("N")}";
        public List<MessageHistory> MessageHistories { get; set; }
        public string LastMessage { get; set; }
        public bool IsPinned { get; set; }
        public ContactType Type { get; set; }
    }
}
