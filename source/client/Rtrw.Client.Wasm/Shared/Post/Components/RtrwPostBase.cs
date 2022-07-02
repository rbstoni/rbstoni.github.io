using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Rtrw.Client.Wasm.Components.Base;

namespace Rtrw.Client.Wasm.Components.Post.Internal
{
    public abstract class RtrwPostBase : RtrwComponentBase
    {
        [Parameter] public EventCallback<MouseEventArgs> OnAvatarClick { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> OnNameClick { get; set; }
    }
}
