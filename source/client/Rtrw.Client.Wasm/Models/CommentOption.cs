using System.ComponentModel.DataAnnotations.Schema;

namespace Rtrw.Client.Wasm.Models
{
    public class CommentOption
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public bool CanDelete { get; set; }
        public bool CanEdit { get; set; }
        public bool CanReact { get; set; } = true;
        public bool CanReply { get; set; } = true;
        public bool CanShare { get; set; }
    }
}
