using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rtrw.Client.Wasm.Components.Base;
using Rtrw.Client.Wasm.Components.Extensions;
using Rtrw.Client.Wasm.Utilities;

namespace Rtrw.Client.Wasm.Components
{
    public partial class RtrwDrawer : RtrwComponentBase, IDisposable
    {

        private Guid _breakpointListenerSubscriptionId;
        int _disposeCount;
        private ElementReference contentRef, drawerRef;
        private DotNetObjectReference<RtrwDrawer> dotNetRef;
        private double height;
        private bool? isOpenWhenLarge = null;
        private int mouseEnterListenerId, mouseLeaveListenerId;
        private bool open, _rtl, isRendered, initial = true, keepInitialState, _fixed = true;

        [Parameter]
        public RenderFragment ChildContent { get; set; }
        [Parameter]
        public bool DisableOverlay { get; set; } = false;
        [Parameter]
        public string Height { get; set; }
        [Parameter]
        public bool Open
        {
            get => open;
            set
            {
                if (open == value)
                {
                    return;
                }
                open = value;
                if (isRendered && initial && !keepInitialState)
                {
                    initial = false;
                }
                if (keepInitialState)
                {
                    keepInitialState = false;
                }
                if (isRendered && value)
                {
                    _ = UpdateHeight();
                }

                DrawerContainer?.FireDrawersChanged();
                OpenChanged.InvokeAsync(open);
            }
        }
        [Parameter] public EventCallback<bool> OpenChanged { get; set; }
        [Parameter]
        public bool PreserveOpenState { get; set; } = false;
        [Parameter]
        public string Variant { get; set; } = "temporary";
        [Parameter]
        public string Width { get; set; }
        protected string Classname =>
        new CssBuilder("rtrw-drawer")
          .AddClass($"rtrw-drawer-open", Open)
          .AddClass($"rtrw-drawer-closed", !Open)
          .AddClass($"rtrw-drawer-initial", initial)
          .AddClass($"rtrw-drawer-{Variant}")
          .AddClass(Class)
        .Build();
        protected string OverlayClass =>
        new CssBuilder("rtrw-drawer-overlay rtrw-overlay-drawer")
          .AddClass($"rtrw-drawer-overlay-open", Open)
          .AddClass($"rtrw-drawer-overlay-{Variant}")
          .AddClass($"rtrw-drawer-overlay-initial", initial)
        .Build();
        protected string Stylename =>
        new StyleBuilder()
            //.AddStyle("width", Width, !string.IsNullOrWhiteSpace(Width) && !Fixed)
            .AddStyle("--rtrw-drawer-width", Width, !string.IsNullOrWhiteSpace(Width) && Variant == "temporary")
            .AddStyle("height", Height, !string.IsNullOrWhiteSpace(Height))
            .AddStyle("--rtrw-drawer-content-height", string.IsNullOrWhiteSpace(Height) ? height.ToPx() : Height)
            .AddStyle("visibility", "hidden", string.IsNullOrWhiteSpace(Height) && height == 0)
            .AddStyle(Style)
        .Build();
        [CascadingParameter] RtrwDrawerContainer DrawerContainer { get; set; }
        private bool OverlayVisible => open && !DisableOverlay && Variant == "temporary";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public virtual void Dispose(bool disposing)
        {
            if (Interlocked.Increment(ref _disposeCount) == 1)
            {
                if (disposing)
                {
                    DrawerContainer?.Remove(this);
                    var toDispose = dotNetRef;
                    dotNetRef = null;
                    toDispose?.Dispose();
                }
            }
        }
        public async Task OnNavigation()
        {
            if (Variant == "temporary")
            {
                await OpenChanged.InvokeAsync(false);
            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await UpdateHeight();
                Console.WriteLine(height);
                isRendered = true;
                if (string.IsNullOrWhiteSpace(Height))
                {
                    StateHasChanged();
                }
            }

            await base.OnAfterRenderAsync(firstRender);
        }
        protected override void OnInitialized()
        {
            if (Variant != "temporary")
            {
                DrawerContainer?.Add(this);
            }
            dotNetRef = DotNetObjectReference.Create(this);
            base.OnInitialized();
        }
        private void CloseDrawer()
        {
            if (Open)
            {
                OpenChanged.InvokeAsync(false);
            }
        }
        private async Task UpdateHeight()
        {
            height = (await contentRef.MudGetBoundingClientRectAsync())?.Height ?? 0;
        }

    }
}

