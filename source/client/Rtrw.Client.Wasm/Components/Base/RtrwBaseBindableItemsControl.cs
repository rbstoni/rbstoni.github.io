using Microsoft.AspNetCore.Components;

namespace Rtrw.Client.Wasm.Components.Base
{
    public abstract class RtrwBaseBindableItemsControl<TChildComponent, TData> : RtrwBaseItemsControl<TChildComponent> where TChildComponent : RtrwComponentBase
    {

        [Parameter] public IEnumerable<TData>? ItemsSource { get; set; }
        [Parameter] public RenderFragment<TData>? ItemTemplate { get; set; }
        public object SelectedItem => ItemsSource == null ? Items[SelectedIndex] : ItemsSource.ElementAtOrDefault(SelectedIndex);

    }
}
