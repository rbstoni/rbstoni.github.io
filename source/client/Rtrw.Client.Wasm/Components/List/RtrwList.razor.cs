using Microsoft.AspNetCore.Components;
using Rtrw.Client.Wasm.Components.Base;
using Rtrw.Client.Wasm.Components.Extensions;
using Rtrw.Client.Wasm.Utilities;

namespace Rtrw.Client.Wasm.Components
{
    public partial class RtrwList : RtrwComponentBase, IDisposable
    {

        private HashSet<RtrwList> childLists = new();
        private HashSet<RtrwListItem> listItems = new();
        private RtrwListItem selectedItem;
        private object selectedValue;

        internal event Action ParametersChanged;

        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public bool Clickable { get; set; }
        [Parameter] public string Color { get; set; } = "primary";
        [Parameter]
        public bool Disabled { get; set; }
        [Parameter]
        public RtrwListItem SelectedItem
        {
            get => selectedItem;
            set
            {
                if (selectedItem == value)
                    return;
                SetSelectedValue(selectedItem?.Value, force: true);
            }
        }
        [Parameter] public EventCallback<RtrwListItem> SelectedItemChanged { get; set; }
        [Parameter]
        public object SelectedValue { get => selectedValue; set { SetSelectedValue(value, force: true); } }
        [Parameter] public EventCallback<object> SelectedValueChanged { get; set; }
        internal bool CanSelect { get; private set; }
        protected string Classname => new CssBuilder("rtrw-list")
          .AddClass(Class)
            .Build();
        [CascadingParameter] protected RtrwList ParentList { get; set; }

        public void Dispose()
        {
            ParametersChanged = null;
            ParentList?.Unregister(this);
        }
        internal void Register(RtrwListItem item)
        {
            listItems.Add(item);
            if (CanSelect && SelectedValue != null && object.Equals(item.Value, SelectedValue))
            {
                item.SetSelected(true);
                selectedItem = item;
                SelectedItemChanged.InvokeAsync(item);
            }
        }
        internal void Register(RtrwList child) => childLists.Add(child);
        internal void SetSelectedValue(object value, bool force = false)
        {
            if ((!CanSelect || !Clickable) && !force)
                return;
            if (object.Equals(selectedValue, value))
                return;
            selectedValue = value;
            SelectedValueChanged.InvokeAsync(value).AndForget();
            selectedItem = null; // <-- for now, we'll see which item matches the value below
            foreach (var listItem in listItems.ToArray())
            {
                var isSelected = value != null && object.Equals(value, listItem.Value);
                listItem.SetSelected(isSelected);
                if (isSelected)
                    selectedItem = listItem;
            }
            foreach (var childList in childLists.ToArray())
            {
                childList.SetSelectedValue(value);
                if (childList.SelectedItem != null)
                    selectedItem = childList.SelectedItem;
            }
            SelectedItemChanged.InvokeAsync(selectedItem).AndForget();
            ParentList?.SetSelectedValue(value);
        }
        internal void Unregister(RtrwListItem item) => listItems.Remove(item);
        internal void Unregister(RtrwList child) => childLists.Remove(child);
        protected override void OnInitialized()
        {
            if (ParentList != null)
            {
                ParentList.Register(this);
                CanSelect = ParentList.CanSelect;
            }
            else
            {
                CanSelect = SelectedItemChanged.HasDelegate || SelectedValueChanged.HasDelegate || SelectedValue != null;
            }
        }
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            ParametersChanged?.Invoke();
        }

    }
}
