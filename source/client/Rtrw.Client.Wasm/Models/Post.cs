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
        public Geocoder? PostLocation { get; set; }
        public Scope Scope { get; set; } = Scope.WargaSekitar;
        public string Text { get; set; }
        public string? TruncatedText => Text.TruncateString(160);
        public string? TopikLiputan { get; set; }
        public string? TopikPengumuman { get; set; }
        public string? Tempat { get; set; }
        public DateTime Tanggal { get; set; }
        public DateTime Waktu { get; set; }
        public bool BersifatUndangan { get; set; }
        public Poll? Poll { get; set; }
        public virtual List<Reaction> Reactions { get; set; } = new();
        public virtual List<Warga> Mentions { get; set; } = new();
        public virtual List<Medium> Media { get; set; } = new();
        public virtual List<Comment> Comments { get; set; } = new();
    }
}
