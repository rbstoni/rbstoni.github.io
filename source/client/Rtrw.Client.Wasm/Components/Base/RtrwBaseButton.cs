using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Rtrw.Client.Wasm.Components.Enums;

namespace Rtrw.Client.Wasm.Components.Base
{
    public abstract class RtrwBaseButton : RtrwComponentBase
    {

        protected ElementReference elementReference;

        [Parameter] public bool Disabled { get; set; }
        [Parameter] public string? Href { get; set; }
        [Parameter] public string HtmlTag { get; set; } = "button";
        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
        [Parameter] public string? Target { get; set; }
        [Parameter] public ButtonType Type { get; set; }

        public ValueTask FocusAsync() => elementReference.FocusAsync();
        protected async Task OnClickHandler(MouseEventArgs ev)
        {
            if (Disabled)
                return;
            await OnClick.InvokeAsync(ev);
        }
        protected override void OnInitialized() => SetDefaultValues();
        protected override void OnParametersSet() => SetDefaultValues();
        private void SetDefaultValues()
        {
            if (Disabled)
            {
                HtmlTag = "button";
                Href = null;
                Target = null;
                return;
            }
            if (!string.IsNullOrWhiteSpace(Href))
                HtmlTag = "a";
        }

    }
}
