using Rtrw.Client.Wasm.Extensions;

namespace Rtrw.Client.Wasm.Models
{
    public class Post
    {
        public string Id { get; set; } = $"post-{Guid.NewGuid().ToString("N")}";
        public Warga Author { get; set; }
        public PostCategory Category { get; set; } = PostCategory.SuaraWarga;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public PostOption? Option { get; set; }
        public Geocoder? PostGeocoder { get; set; }
        public Scope Scope { get; set; } = Scope.WargaSekitar;
        public string Text { get; set; }
        public string? TruncatedText => Text.TruncateString(160);
        public Poll? Poll { get; set; }
        public virtual List<Reaction> Reactions { get; set; } = new();
        public virtual List<Warga> Mentions { get; set; } = new();
        public virtual List<Medium> Media { get; set; } = new();
        public virtual List<Comment> Comments { get; set; } = new();
    }
}
