﻿namespace Rtrw.Client.Wasm.Models
{
    public class Notification
    {
        public string Id { get; set; }
        public Warga Recipient { get; set; }
        public Warga? Notifier { get; set; }
        public DateTime NotifiedAt { get; set; }
        public bool Read { get; set; }
        public string ContentUri { get; set; }
    }
}
