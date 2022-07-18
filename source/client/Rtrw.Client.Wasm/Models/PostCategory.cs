using System.ComponentModel;

namespace Rtrw.Client.Wasm.Models
{
    public enum PostCategory
    {
        [Description("Umum")]
        Umum,
        [Description("Bisnis")]
        Bisnis,
        [Description("Keamanan-Ketertiban")]
        KeamananKetertiban,
        [Description("Kebersihan")]
        Kebersihan
    }
}
