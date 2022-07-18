using System.ComponentModel.DataAnnotations.Schema;

namespace Rtrw.Client.Wasm.Models
{
    public class Registration
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public Warga Warga { get; set; }
        public int CurrentStep { get; set; }
        public int CurrentProgress { get; set; }
        public bool Finished { get; set; }
    }
}
