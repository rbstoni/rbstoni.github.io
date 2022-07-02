using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Rtrw.Client.Wasm.Components.Base;

namespace Rtrw.Client.Wasm.Components
{
    public class RtrwElement : RtrwComponentBase
    {

        [Parameter]
        public RenderFragment ChildContent { get; set; }
        [Parameter]
        public string HtmlTag { get; set; } = "div";
        [Parameter]
        public ElementReference? Ref { get; set; }
        [Parameter] public EventCallback<ElementReference> RefChanged { get; set; }

        public void Refresh() => StateHasChanged();
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);
            builder.OpenElement(0, HtmlTag);
            foreach (var attribute in UserAttributes)
                if (attribute.Value != null)
                    builder.AddAttribute(1, attribute.Key, attribute.Value);
            builder.AddAttribute(2, "class", Class);
            builder.AddAttribute(3, "style", Style);
            if (HtmlTag == "button")
                builder.AddEventStopPropagationAttribute(5, "onclick", true);
            if (Ref != null)
                builder.AddElementReferenceCapture(6, async capturedRef =>
                {
                    Ref = capturedRef;
                    await RefChanged.InvokeAsync(Ref.Value);
                });
            builder.AddContent(10, ChildContent);
            builder.CloseElement();
        }

    }
}
