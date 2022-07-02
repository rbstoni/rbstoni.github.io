using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Rtrw.Client.Wasm.Components.Base;
using Rtrw.Client.Wasm.Utilities;

namespace Rtrw.Client.Wasm.Components
{
    public partial class RtrwListItem : RtrwComponentBase, IDisposable
    {

        private bool disabled;
        private bool expanded;
        private bool onClickHandlerPreventDefault = false;
        private bool selected;

        [Parameter]
        public RenderFragment ChildContent { get; set; }
        [Parameter]
        public bool Disabled
        {
            get => disabled || (RtrwList?.Disabled ?? false);
            set => disabled = value;
        }
        [Parameter]
        public bool Expanded
        {
            get => expanded;
            set
            {
                if (expanded == value)
                    return;
                expanded = value;
                _ = ExpandedChanged.InvokeAsync(value);
            }
        }
        [Parameter]
        public EventCallback<bool> ExpandedChanged { get; set; }
        [Parameter]
        public bool ForceLoad { get; set; }
        [Parameter]
        public string Href { get; set; }
        [Parameter]
        public bool InitiallyExpanded { get; set; }
        [Parameter]
        public RenderFragment NestedList { get; set; }
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }
        [Parameter]
        public bool OnClickHandlerPreventDefault
        {
            get => onClickHandlerPreventDefault;
            set => onClickHandlerPreventDefault = value;
        }
        [Parameter]
        public object Value { get; set; }
        protected string Classname =>
            new CssBuilder("rtrw-list-item")
                .AddClass("rtrw-list-item-clickable", RtrwList?.Clickable)
                .AddClass($"rtrw-selected-item rtrw-{RtrwList?.Color}-text rtrw-{RtrwList?.Color}-hover", selected && !Disabled)
                .AddClass("rtrw-list-item-disabled", Disabled)
                .AddClass(Class)
                .Build();
        [CascadingParameter] protected RtrwList RtrwList { get; set; }
        [Inject] protected NavigationManager UriHelper { get; set; }

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
            if (selected == selected)
                return;
            selected = selected;
            StateHasChanged();
        }
        protected void OnClickHandler(MouseEventArgs ev)
        {
            if (Disabled)
                return;
            if (!onClickHandlerPreventDefault)
            {
                if (NestedList != null)
                {
                    Expanded = !Expanded;
                }
                else if (Href != null)
                {
                    RtrwList?.SetSelectedValue(this.Value);
                    OnClick.InvokeAsync(ev);
                    UriHelper.NavigateTo(Href, ForceLoad);
                }
                else
                {
                    RtrwList?.SetSelectedValue(this.Value);
                    OnClick.InvokeAsync(ev);
                }
            }
            else
            {
                OnClick.InvokeAsync(ev);
            }
        }
        protected override void OnInitialized()
        {
            expanded = InitiallyExpanded;
            if (RtrwList != null)
            {
                RtrwList.Register(this);
                OnListParametersChanged();
                RtrwList.ParametersChanged += OnListParametersChanged;
            }
        }
        private void OnListParametersChanged()
        {
            StateHasChanged();
        }

    }
}
