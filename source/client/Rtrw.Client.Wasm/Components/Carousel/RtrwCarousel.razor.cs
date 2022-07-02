using Microsoft.AspNetCore.Components;
using Rtrw.Client.Wasm.Components.Base;
using Rtrw.Client.Wasm.Components.Enums;
using Rtrw.Client.Wasm.Utilities;

namespace Rtrw.Client.Wasm.Components
{
    public partial class RtrwCarousel<TData> : RtrwBaseBindableItemsControl<RtrwCarouselItem, TData>, IAsyncDisposable
    {
        protected string Classname =>
            new CssBuilder("rtrw-carousel")
                .AddClass(Class)
                .Build();

        [Parameter] public bool ShowArrows { get; set; } = true;

        [Parameter] public bool ShowBullets { get; set; } = true;

        public override void AddItem(RtrwCarouselItem item)
        {
            Items.Add(item);
            if (Items.Count - 1 == SelectedIndex)
            {
                StateHasChanged();
            }
        }

        private void OnSwipe(SwipeDirection direction)
        {
            switch (direction)
            {
                case SwipeDirection.LeftToRight:
                    Previous();
                    break;

                case SwipeDirection.RightToLeft:
                    Next();
                    break;
            }
        }


        public async ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
        }
    }
}
