using Microsoft.AspNetCore.Components;
using Rtrw.Client.Wasm.Components.Base;
using Rtrw.Client.Wasm.Components.Enums;
using Rtrw.Client.Wasm.Components.Extensions;
using Rtrw.Client.Wasm.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Components
{
    public partial class RtrwList: RtrwComponentBase, IDisposable
    {
        private HashSet<RtrwList> _childLists = new();
        private HashSet<RtrwListItem> _items = new();
        private RtrwListItem? _selectedItem;
        private object? _selectedValue;
        internal event Action? ParametersChanged;
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public bool Clickable { get; set; }
        [Parameter] public ThemeColor Color { get; set; } = ThemeColor.Primary;
        [Parameter] public bool Dense { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public bool DisableGutters { get; set; }
        [Parameter] public bool DisablePadding { get; set; }
        [Parameter]
        public RtrwListItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem == value)
                    return;
                SetSelectedValue(_selectedItem?.Value, force: true);
            }
        }
        [Parameter] public EventCallback<RtrwListItem> SelectedItemChanged { get; set; }
        [Parameter]
        public object SelectedValue
        {
            get => _selectedValue;
            set
            {
                SetSelectedValue(value, force: true);
            }
        }
        [Parameter] public EventCallback<object> SelectedValueChanged { get; set; }
        internal bool CanSelect { get; private set; }
        protected string Classname
            => new CssBuilder("rtrw-list")
                .AddClass("rtrw-list-padding", !DisablePadding)
                .AddClass(Class ?? string.Empty)
                .Build();
        [CascadingParameter] protected RtrwList? ParentList { get; set; }

        public void Dispose()
        {
            ParametersChanged = null;
            ParentList?.Unregister(this);
        }

        internal void Register(RtrwListItem item)
        {
            _items.Add(item);
            if (CanSelect && SelectedValue != null && Equals(item.Value, SelectedValue))
            {
                item.SetSelected(true);
                _selectedItem = item;
                SelectedItemChanged.InvokeAsync(item);
            }
        }

        internal void Register(RtrwList child)
        {
            _childLists.Add(child);
        }

        internal void SetSelectedValue(object value, bool force = false)
        {
            if ((!CanSelect || !Clickable) && !force)
                return;
            if (Equals(_selectedValue, value))
                return;
            _selectedValue = value;
            SelectedValueChanged.InvokeAsync(value).AndForget();
            _selectedItem = null; // <-- for now, we'll see which item matches the value below
            foreach (var listItem in _items.ToArray())
            {
                var isSelected = value != null && Equals(value, listItem.Value);
                listItem.SetSelected(isSelected);
                if (isSelected)
                    _selectedItem = listItem;
            }
            foreach (var childList in _childLists.ToArray())
            {
                childList.SetSelectedValue(value);
                if (childList.SelectedItem != null)
                    _selectedItem = childList.SelectedItem;
            }
            SelectedItemChanged.InvokeAsync(_selectedItem).AndForget();
            ParentList?.SetSelectedValue(value);
        }

        internal void Unregister(RtrwListItem item) => _items.Remove(item);

        internal void Unregister(RtrwList child) => _childLists.Remove(child);

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
