using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Rtrw.Client.Wasm.Components.Base
{
    public class RtrwBaseSelectItem : RtrwComponentBase
    {

        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public ICommand Command { get; set; }
        [Parameter] public object CommandParameter { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public bool DisableRipple { get; set; }
        [Parameter] public bool ForceLoad { get; set; }
        [Parameter] public string Href { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
        [Inject] private NavigationManager UriHelper { get; set; }

        protected async Task OnClickHandler(MouseEventArgs ev)
        {
            if (Disabled)
                return;
            if (Href != null)
            {
                UriHelper.NavigateTo(Href, ForceLoad);
            }
            else
            {
                await OnClick.InvokeAsync(ev);
                if (Command?.CanExecute(CommandParameter) ?? false)
                {
                    Command.Execute(CommandParameter);
                }
            }
        }

    }
}
