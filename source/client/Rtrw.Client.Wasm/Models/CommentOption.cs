namespace Rtrw.Client.Wasm.Models
{
    public class CommentOption
    {
        public string Id { get; set; }
        public bool CanDelete { get; set; }
        public bool CanEdit { get; set; }
        public bool CanReact { get; set; } = true;
        public bool CanReply { get; set; } = true;
        public bool CanShare { get; set; }
    }
}
