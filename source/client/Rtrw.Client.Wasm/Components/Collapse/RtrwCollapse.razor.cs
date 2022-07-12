using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rtrw.Client.Wasm.Components.Base;
using Rtrw.Client.Wasm.Components.Extensions;
using Rtrw.Client.Wasm.Utilities;
using System.Globalization;

namespace Rtrw.Client.Wasm.Components
{
    public partial class RtrwCollapse : RtrwComponentBase, IDisposable
    {
        internal enum CollapseState
        {
            Entering, Entered, Exiting, Exited
        }
        internal double height;
        private int listenerId;
        private bool expanded, isRendered, updateHeight;
        private ElementReference container, wrapper;
        internal CollapseState state = CollapseState.Exited;
        private DotNetObjectReference<RtrwCollapse> dotNetRef;
        protected string Stylename =>
            new StyleBuilder()
            .AddStyle("max-height", MaxHeight.ToPx(), MaxHeight != null)
            .AddStyle("height", "auto", state == CollapseState.Entered)
            .AddStyle("height", height.ToPx(), state is CollapseState.Entering or CollapseState.Exiting)
            .AddStyle("animation-duration", $"{CalculatedAnimationDuration.ToString("#.##", CultureInfo.InvariantCulture)}s", state == CollapseState.Entering)
            .AddStyle(Style)
            .Build();
        protected string Classname =>
            new CssBuilder("rtrw-collapse-container")
            .AddClass($"rtrw-collapse-entering", state == CollapseState.Entering)
            .AddClass($"rtrw-collapse-entered", state == CollapseState.Entered)
            .AddClass($"rtrw-collapse-exiting", state == CollapseState.Exiting)
            .AddClass(Class)
            .Build();
        [Parameter]
        public bool Expanded
        {
            get => expanded;
            set
            {
                if (expanded == value)
                    return;
                expanded = value;

                if (isRendered)
                {
                    state = expanded ? CollapseState.Entering : CollapseState.Exiting;
                    _ = UpdateHeight();
                    updateHeight = true;
                }
                else if (expanded)
                {
                    state = CollapseState.Entered;
                }

                _ = ExpandedChanged.InvokeAsync(expanded);
            }
        }
        [Parameter] public int? MaxHeight { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public EventCallback OnAnimationEnd { get; set; }
        [Parameter] public EventCallback<bool> ExpandedChanged { get; set; }
        private double CalculatedAnimationDuration
        {
            get
            {
                if (MaxHeight != null)
                {
                    if (MaxHeight <= 200) return 0.2;
                    else if (MaxHeight <= 600) return 0.4;
                    else if (MaxHeight <= 1400) return 0.6;
                    return 1;
                }
                return Math.Min(height / 800.0, 1);
            }
            set { }
        }
        internal async Task UpdateHeight()
        {
            if (disposeCount > 0)
            {
                height = 0;
            }
            else
            {
                height = (await wrapper.RtrwGetBoundingClientRectAsync())?.Height ?? 0;
            }

            if (MaxHeight != null && height > MaxHeight)
            {
                height = MaxHeight.Value;
            }
        }
        protected override void OnInitialized()
        {
            dotNetRef = DotNetObjectReference.Create(this);
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                isRendered = true;
                await UpdateHeight();
                listenerId = await container.RtrwAddEventListenerAsync(dotNetRef, "animationend", nameof(AnimationEnd));
            }
            else if (updateHeight && state is CollapseState.Entering or CollapseState.Exiting)
            {
                updateHeight = false;
                await UpdateHeight();
                StateHasChanged();
            }
            await base.OnAfterRenderAsync(firstRender);
        }
        internal int disposeCount;
        protected virtual void Dispose(bool disposing)
        {
            if (Interlocked.Increment(ref disposeCount) == 1)
            {
                if (disposing)
                {
                    if (listenerId != 0)
                        _ = container.RtrwRemoveEventListenerAsync("animationend", listenerId);
                    var toDispose = dotNetRef;
                    dotNetRef = null;
                    toDispose?.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [JSInvokable]
        public void AnimationEnd()
        {
            if (state == CollapseState.Entering)
            {
                state = CollapseState.Entered;
                StateHasChanged();
            }
            else if (state == CollapseState.Exiting)
            {
                state = CollapseState.Exited;
                StateHasChanged();
            }
            OnAnimationEnd.InvokeAsync(Expanded);
        }
    }
}
