namespace Rtrw.Client.Wasm.Models
{
    public class FeedItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public Warga FeedOwner { get; set; }
        public List<Post> FeedPosts { get; set; }
    }
}
