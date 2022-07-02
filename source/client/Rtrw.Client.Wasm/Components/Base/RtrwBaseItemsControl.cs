using Microsoft.AspNetCore.Components;

namespace Rtrw.Client.Wasm.Components.Base
{
    public partial class RtrwBaseItemsControl<TChildComponent> : RtrwComponentBase where TChildComponent : RtrwComponentBase
    {

        internal bool moveNext = true;
        private int selectedIndexField = -1;

        [Parameter]
        public RenderFragment ChildContent { get; set; }
        public List<TChildComponent> Items { get; } = new List<TChildComponent>();
        public TChildComponent LastContainer { get; private set; } = null;
        public TChildComponent SelectedContainer
        {
            get => SelectedIndex >= 0 && Items.Count > SelectedIndex ? Items[SelectedIndex] : null;
        }
        [Parameter]
        public int SelectedIndex
        {
            get => selectedIndexField;
            set
            {
                if (SelectedIndex == value)
                    return;

                LastContainer = selectedIndexField >= 0 ? SelectedContainer : null;
                selectedIndexField = value;
                SelectionChanged();
                StateHasChanged();
                SelectedIndexChanged.InvokeAsync(value);
            }
        }
        [Parameter]
        public EventCallback<int> SelectedIndexChanged { get; set; }

        public virtual void AddItem(TChildComponent item) { }
        public void MoveTo(int index)
        {
            if (SelectedIndex != index)
            {
                moveNext = index >= SelectedIndex;
                SelectedIndex = index;
            }
        }
        public void Next()
        {
            moveNext = true;

            if (SelectedIndex < (Items.Count - 1))
                SelectedIndex++;
            else
                SelectedIndex = 0;
        }
        public void Previous()
        {
            moveNext = false;

            if (SelectedIndex > 0)
                SelectedIndex--;
            else
                SelectedIndex = Items.Count - 1;
        }
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (Items.Count > 0 && SelectedIndex < 0)
                    MoveTo(0);

            }
            return base.OnAfterRenderAsync(firstRender);
        }
        protected virtual void SelectionChanged() { }

    }
}
