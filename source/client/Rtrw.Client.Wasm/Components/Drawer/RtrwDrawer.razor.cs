using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rtrw.Client.Wasm.Components.Base;
using Rtrw.Client.Wasm.Components.Enums;
using Rtrw.Client.Wasm.Components.Extensions;
using Rtrw.Client.Wasm.Utilities;

namespace Rtrw.Client.Wasm.Components
{
    public partial class RtrwDrawer : RtrwComponentBase, IDisposable
    {

        int _disposeCount;
        private ElementReference _contentRef, _drawerRef;
        private DotNetObjectReference<RtrwDrawer> dotNetRef;
        private double height;
        private int _mouseEnterListenerId, _mouseLeaveListenerId;
        private bool _open, _isRendered, _initial = true, _keepInitialState, _fixed = true;
        private DrawerClipMode _clipMode;
        private bool closeOnMouseLeave = false;


        [Parameter] public Anchor Anchor { get; set; } = Anchor.Bottom;
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public bool DisableOverlay { get; set; } = false;
        [Parameter] public string Height { get; set; }
        [Parameter] public string MiniWidth { get; set; }
        [Parameter]
        public bool Fixed
        {
            get => _fixed && DrawerContainer is RtrwLayout; set
            {
                if (_fixed == value)
                    return;
                _fixed = value;
            }
        }
        [Parameter]
        public DrawerClipMode ClipMode
        {
            get => _clipMode;
            set
            {
                if (_clipMode == value)
                    return;
                _clipMode = value;
                if (Fixed)
                {
                    DrawerContainer?.FireDrawersChanged();
                }
                StateHasChanged();
            }
        }
        [Parameter]
        public bool Open
        {
            get => _open;
            set
            {
                if (_open == value)
                    return;
                _open = value;
                if (_isRendered && _initial && !_keepInitialState)
                    _initial = false;
                if (_keepInitialState)
                    _keepInitialState = false;
                if (_isRendered && value && (Anchor == Anchor.Top || Anchor == Anchor.Bottom))
                {
                    _ = UpdateHeight();
                }

                DrawerContainer?.FireDrawersChanged();
                OpenChanged.InvokeAsync(_open);
            }
        }
        [Parameter] public EventCallback<bool> OpenChanged { get; set; }
        [Parameter] public bool OpenMiniOnHover { get; set; }
        [Parameter]
        public bool PreserveOpenState { get; set; } = false;
        [Parameter]
        public DrawerVariant Variant { get; set; } = DrawerVariant.Temporary;
        [Parameter]
        public string Width { get; set; }
        protected string Classname
            => new CssBuilder("rtrw-drawer")
                .AddClass("rtrw-drawer-fixed", Fixed)
                .AddClass($"rtrw-drawer-pos-{Anchor.EnumToDescriptionString()}")
                .AddClass("rtrw-drawer-open", Open)
                .AddClass("rtrw-drawer-closed", !Open)
                .AddClass("rtrw-drawer-initial", _initial)
                .AddClass($"rtrw-drawer-clipped-{_clipMode.EnumToDescriptionString()}")
                .AddClass($"rtrw-drawer-{Variant.EnumToDescriptionString()}")
                .AddClass(Class)
                .Build();

        protected string OverlayClass
            => new CssBuilder("rtrw-drawer-overlay rtrw-overlay-drawer")
                .AddClass($"mud-drawer-pos-{Anchor.EnumToDescriptionString()}")
                .AddClass($"rtrw-drawer-overlay-open", Open)
                .AddClass($"rtrw-drawer-overlay-{Variant}")
                .AddClass($"rtrw-drawer-overlay-initial", _initial)
                .Build();
        protected string Stylename
            => new StyleBuilder()
                .AddStyle("--rtrw-drawer-width", Width, !string.IsNullOrWhiteSpace(Width) && Variant == DrawerVariant.Temporary)
                .AddStyle("height", Height, !string.IsNullOrWhiteSpace(Height))
                .AddStyle("--rtrw-drawer-content-height", string.IsNullOrWhiteSpace(Height) ? height.ToPx() : Height, Anchor == Anchor.Bottom || Anchor == Anchor.Top)
                .AddStyle("visibility", "hidden", string.IsNullOrWhiteSpace(Height) && height == 0 && (Anchor == Anchor.Bottom || Anchor == Anchor.Top))
                .AddStyle(Style)
                .Build();
        [CascadingParameter] RtrwDrawerContainer DrawerContainer { get; set; }
        private bool OverlayVisible
            => _open && !DisableOverlay && (Variant == DrawerVariant.Temporary || Variant == DrawerVariant.Mini);
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
                    if (_mouseEnterListenerId != 0)
                        _ = _drawerRef.RtrwRemoveEventListenerAsync("mouseenter", _mouseEnterListenerId);
                    if (_mouseLeaveListenerId != 0)
                        _ = _drawerRef.RtrwRemoveEventListenerAsync("mouseleave", _mouseLeaveListenerId);
                    var toDispose = dotNetRef;
                    dotNetRef = null;
                    toDispose?.Dispose();
                }
            }
        }
        [JSInvokable]
        public async void OnMouseEnter()
        {
            if (Variant == DrawerVariant.Mini && !Open && OpenMiniOnHover)
            {
                closeOnMouseLeave = true;
                await OpenChanged.InvokeAsync(true);
            }
        }
        [JSInvokable]
        public async void OnMouseLeave()
        {
            if (Variant == DrawerVariant.Mini && Open && closeOnMouseLeave)
            {
                closeOnMouseLeave = false;
                await OpenChanged.InvokeAsync(false);
            }
        }
        public async Task OnNavigation()
        {
            if (Variant == DrawerVariant.Temporary)
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
                _isRendered = true;
                if (string.IsNullOrWhiteSpace(Height) && (Anchor == Anchor.Bottom || Anchor == Anchor.Top))
                {
                    StateHasChanged();
                }
                _mouseEnterListenerId = await _drawerRef.RtrwAddEventListenerAsync(dotNetRef, "mouseenter", nameof(OnMouseEnter), true);
                _mouseLeaveListenerId = await _drawerRef.RtrwAddEventListenerAsync(dotNetRef, "mouseleave", nameof(OnMouseLeave), true);
            }

            await base.OnAfterRenderAsync(firstRender);
        }
        protected override void OnInitialized()
        {
            if (Variant != DrawerVariant.Temporary)
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
            height = (await _contentRef.RtrwGetBoundingClientRectAsync())?.Height ?? 0;
        }

    }
}

