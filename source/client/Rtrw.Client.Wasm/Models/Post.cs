using Rtrw.Client.Wasm.Extensions;

namespace Rtrw.Client.Wasm.Models
{
    public class Post
    {
        public string Id { get; set; } = $"post-{Guid.NewGuid():N}";
        public Warga Author { get; set; }
        public PostCategory Category { get; set; } = PostCategory.Umum;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public PostOption? Option { get; set; }
        public Geocoder? PostGeocoder { get; set; }
        public Scope Scope { get; set; } = Scope.WargaSekitar;
        public string Text { get; set; }
        public Poll? Poll { get; set; }
        public List<Report>? Reports { get; set; }
        public List<Reaction> Reactions { get; set; } = new();
        public List<Warga> Mentions { get; set; } = new();
        public List<Medium> Media { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
    }
}
