
using Rtrw.Client.Wasm.Extensions;

namespace Rtrw.Client.Wasm.Models
{
    public class Comment
    {
        public string Id { get; set; } = $"comment-{Guid.NewGuid().ToString("N")}";
        public Warga Commenter { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime EditedAt { get; set; }
        public string? CommentUrl { get; set; }
        public string Text { get; set; }
        public string TruncatedText => Text.TruncateString(160);
        public List<Report>? Reports { get; set; }
        public List<Medium>? Media { get; set; } = new();
        public List<Reaction>? Reactions { get; set; } = new();
        public List<Warga>? Mentions { get; set; } = new();
        public List<Comment>? Replies { get; set; } = new();
        public CommentOption? Option { get; set; }
    }
}
