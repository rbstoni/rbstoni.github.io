using Microsoft.AspNetCore.Components;
using Rtrw.Client.Wasm.Components.Base;
using Rtrw.Client.Wasm.Components.Input;
using Rtrw.Client.Wasm.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Components
{
    public partial class RtrwSelectItem<T> : RtrwBaseSelectItem, IDisposable
    {

        private bool _isSelected;
        private IRtrwSelect? _parent;
        private IRtrwShadowSelect? _shadowParent;

        [Parameter] public T? Value { get; set; }
        [CascadingParameter(Name = "HideContent")]
        internal bool HideContent { get; set; }
        [CascadingParameter]
        internal IRtrwSelect IRtrwSelect
        {
            get => _parent ?? null!;
            set
            {
                _parent = value;
                if (_parent == null)
                    return;
                _parent.CheckGenericTypeMatch(this);
                if (RtrwSelect == null)
                    return;
                bool isSelected = RtrwSelect.Add(this);
                if (_parent.MultiSelection)
                {
                    RtrwSelect.SelectionChangedFromOutside += OnUpdateSelectionStateFromOutside;
                    InvokeAsync(() => OnUpdateSelectionStateFromOutside(RtrwSelect.SelectedValues));
                }
                else
                {
                    IsSelected = isSelected;
                }
            }
        }
        [CascadingParameter]
        internal IRtrwShadowSelect IRtrwShadowSelect
        {
            get => _shadowParent ?? null!;
            set
            {
                _shadowParent = value;
                ((RtrwSelect<T>)_shadowParent)?.RegisterShadowItem(this);
            }
        }
        internal bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
            }
        }
        internal string ItemId { get; } = string.Concat("_", Guid.NewGuid().ToString().AsSpan(0, 8));
        internal RtrwSelect<T> RtrwSelect => (RtrwSelect<T>)IRtrwSelect;
        protected string DisplayString
        {
            get
            {
                var converter = RtrwSelect?.Converter;
                if (converter == null)
                    return $"{Value}";
                return converter.Set(Value ?? default!);
            }
        }
        protected bool MultiSelection
        {
            get
            {
                if (RtrwSelect == null)
                    return false;
                return RtrwSelect.MultiSelection;
            }
        }

        public void Dispose()
        {
            try
            {
                RtrwSelect?.Remove(this);
                ((RtrwSelect<T>?)_shadowParent)?.UnregisterShadowItem(this);
            }
            catch (Exception) { }
        }
        protected string? GetCssClasses()
            => new CssBuilder()
                .AddClass(Class ?? default!)
                .Build();
        private void OnClicked()
        {
            if (MultiSelection)
                IsSelected = !IsSelected;

            RtrwSelect?.SelectOption(Value ?? default!);
            InvokeAsync(StateHasChanged);
        }
        private void OnUpdateSelectionStateFromOutside(IEnumerable<T> selection)
        {
            if (selection == null)
                return;
            var old_is_selected = IsSelected;
            IsSelected = selection.Contains(Value);
            if (old_is_selected != IsSelected)
                InvokeAsync(StateHasChanged);
        }

    }
}
