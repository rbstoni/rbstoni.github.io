using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Rtrw.Client.Wasm.Components.Base;
using Rtrw.Client.Wasm.Components.Enums;
using Rtrw.Client.Wasm.Components.Extensions;
using Rtrw.Client.Wasm.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Rtrw.Client.Wasm.Components
{
    public partial class RtrwListItem : RtrwComponentBase, IDisposable
    {
        private bool _disabled;
        private bool _expanded;
        private bool _onClickHandlerPreventDefault = false;
        private bool _selected;
        private Typo _textTypo;

        [Parameter] public ThemeColor AdornmentColor { get; set; } = ThemeColor.Default;
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public int? IconSize { get; set; }
        [Parameter] public ICommand? Command { get; set; }
        [Parameter] public object? CommandParameter { get; set; }
        [Parameter] public bool Dense { get; set; }
        [Parameter]
        public bool Disabled
        {
            get => _disabled || (RtrwList?.Disabled ?? false);
            set => _disabled = value;
        }
        [Parameter] public bool DisableGutters { get; set; }
        [Parameter] public bool DisableRipple { get; set; }
        [Parameter]
        public bool Expanded
        {
            get => _expanded;
            set
            {
                if (_expanded == value)
                    return;
                _expanded = value;
                _ = ExpandedChanged.InvokeAsync(value);
            }
        }
        [Parameter] public EventCallback<bool> ExpandedChanged { get; set; }
        [Parameter] public bool ForceLoad { get; set; }
        [Parameter] public string? Href { get; set; }
        [Parameter] public ThemeColor IconColor { get; set; } = ThemeColor.Inherit;
        [Parameter] public bool InitiallyExpanded { get; set; }
        [Parameter] public bool Inset { get; set; }
        [Parameter] public RenderFragment? NestedList { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
        [Parameter]
        public bool OnClickHandlerPreventDefault
        {
            get => _onClickHandlerPreventDefault;
            set => _onClickHandlerPreventDefault = value;
        }
        [Parameter] public string? Text { get; set; }
        [Parameter] public object? Value { get; set; }
        protected string ClassName
            => new CssBuilder("rtrw-list-item")
                .AddClass("rtrw-list-item-dense", Dense || RtrwList?.Dense == true)
                .AddClass("rtrw-list-item-gutters", !DisableGutters && !(RtrwList?.DisableGutters == true))
                .AddClass("rtrw-list-item-clickable", RtrwList?.Clickable)
                .AddClass("rtrw-ripple", RtrwList?.Clickable == true && !DisableRipple && !Disabled)
                .AddClass($"rtrw-selected-item rtrw-{RtrwList?.Color.EnumToDescriptionString()}-text rtrw-{RtrwList?.Color.EnumToDescriptionString()}-hover", _selected && !Disabled)
                .AddClass("rtrw-list-item-disabled", Disabled)
                .AddClass(Class)
                .Build();
        [CascadingParameter] protected RtrwList? RtrwList { get; set; }
        [Inject] protected NavigationManager? UriHelper { get; set; }

        public void Dispose()
        {
            try
            {
                if (RtrwList == null)
                    return;
                RtrwList.ParametersChanged -= OnListParametersChanged;
                RtrwList.Unregister(this);
            }
            catch (Exception) { /*ignore*/ }
        }

        internal void SetSelected(bool selected)
        {
            if (Disabled)
                return;
            if (_selected == selected)
                return;
            _selected = selected;
            StateHasChanged();
        }

        protected void OnClickHandler(MouseEventArgs ev)
        {
            if (Disabled)
                return;
            if (!_onClickHandlerPreventDefault)
            {
                if (NestedList != null)
                {
                    Expanded = !Expanded;
                }
                else if (Href != null)
                {
                    RtrwList?.SetSelectedValue(this.Value!);
                    OnClick.InvokeAsync(ev);
                    UriHelper?.NavigateTo(Href, ForceLoad);
                }
                else
                {
                    RtrwList?.SetSelectedValue(this.Value!);
                    OnClick.InvokeAsync(ev);
                    if (Command?.CanExecute(CommandParameter) ?? false)
                    {
                        Command.Execute(CommandParameter);
                    }
                }
            }
            else
            {
                OnClick.InvokeAsync(ev);
            }
        }

        protected override void OnInitialized()
        {
            _expanded = InitiallyExpanded;
            if (RtrwList != null)
            {
                RtrwList.Register(this);
                OnListParametersChanged();
                RtrwList.ParametersChanged += OnListParametersChanged;
            }
        }

        private void OnListParametersChanged()
        {
            if (Dense || RtrwList?.Dense == true)
            {
                _textTypo = Typo.Body2;
            }
            else if (!Dense || !RtrwList?.Dense == true)
            {
                _textTypo = Typo.Body1;
            }
            StateHasChanged();
        }
    }
}
