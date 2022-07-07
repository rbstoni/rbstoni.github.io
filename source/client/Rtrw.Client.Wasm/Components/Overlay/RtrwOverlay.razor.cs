using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Rtrw.Client.Wasm.Components.Base;
using Rtrw.Client.Wasm.Components.Services.Scroll;
using Rtrw.Client.Wasm.Utilities;

namespace Rtrw.Client.Wasm.Components
{
    public partial class RtrwOverlay : RtrwComponentBase, IDisposable
    {
        private bool visible;

        protected string Classname =>
            new CssBuilder("rtrw-overlay")
                .AddClass("rtrw-overlay-absolute", Absolute)
                .AddClass(Class)
                .Build();

        protected string ScrimClassname =>
            new CssBuilder("rtrw-overlay-scrim")
                .AddClass("rtrw-overlay-dark", DarkBackground)
                .AddClass("rtrw-overlay-light", LightBackground)
                .Build();

        protected string Styles =>
            new StyleBuilder()
            .AddStyle("z-index", $"{ZIndex}", ZIndex != 5)
            .AddStyle(Style)
            .Build();

        [Inject] public IScrollManager? ScrollManager { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public EventCallback<bool> VisibleChanged { get; set; }
        [Parameter]
        public bool Visible
        {
            get => visible;
            set
            {
                if (visible == value)
                    return;
                visible = value;
                VisibleChanged.InvokeAsync(visible);
            }
        }
        [Parameter] public bool AutoClose { get; set; }
        [Parameter] public bool LockScroll { get; set; } = true;
        [Parameter] public string LockScrollClass { get; set; } = "scroll-locked";
        [Parameter] public bool DarkBackground { get; set; }
        [Parameter] public bool LightBackground { get; set; }
        [Parameter] public bool Absolute { get; set; }
        [Parameter] public int ZIndex { get; set; } = 5;
        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
        protected internal void OnClickHandler(MouseEventArgs ev)
        {
            if (AutoClose)
                Visible = false;
            OnClick.InvokeAsync(ev);
        }

        protected override void OnAfterRender(bool firstTime)
        {
            if (!LockScroll || Absolute)
                return;

            if (Visible)
                BlockScroll();
            else
                UnblockScroll();
        }

        //locks the scroll attaching a CSS class to the specified element, in this case the body
        void BlockScroll()
        {
            ScrollManager.LockScrollAsync("body", LockScrollClass);
        }

        //removes the CSS class that prevented scrolling
        void UnblockScroll()
        {
            ScrollManager.UnlockScrollAsync("body", LockScrollClass);
        }

        //When disposing the overlay, remove the class that prevented scrolling
        public void Dispose()
        {
            UnblockScroll();
        }
    }
}
