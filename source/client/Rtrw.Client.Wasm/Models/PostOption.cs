namespace Rtrw.Client.Wasm.Models
{
    public class PostOption
    {
        public string Id { get; set; } = $"postOption-{Guid.NewGuid().ToString("N")}";
        public bool CanDelete { get; set; }
        public bool CanEdit { get; set; }
        public bool CanReact { get; set; } = true;
        public bool CanReply { get; set; } = true;
        public bool CanShare { get; set; } = true;
    }
}

