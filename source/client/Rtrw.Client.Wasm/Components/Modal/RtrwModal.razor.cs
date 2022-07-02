using Microsoft.AspNetCore.Components;
using Rtrw.Client.Wasm.Components.Base;
using Rtrw.Client.Wasm.Utilities;

namespace Rtrw.Client.Wasm.Components
{
    public partial class RtrwModal : RtrwComponentBase
    {
        protected string ContentClass =>
            new CssBuilder("rtrw-modal-content")
                .AddClass(Class)
                .Build();
        protected string FooterClass =>
            new CssBuilder("rtrw-modal-footer")
                .Build();

        [CascadingParameter] private RtrwModalInstance ModalInstance { get; set; }

        [Inject] public IModalService ModalService { get; set; }

        [Parameter]
        public RenderFragment TitleContent { get; set; }
        [Parameter]
        public RenderFragment ModalContent { get; set; }
        [Parameter]
        public RenderFragment FooterContent { get; set; }
        [Parameter]
        public ModalOptions Options { get; set; }
        [Parameter]
        public Action OnBackdropClick { get; set; }
        [Parameter]
        public string ContentStyle { get; set; }
        [Parameter]
        public bool IsVisible
        {
            get => isVisible;
            set
            {
                if (isVisible == value)
                    return;
                isVisible = value;
                IsVisibleChanged.InvokeAsync(value);
            }
        }
        private bool isVisible;
        [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }
        private bool IsInline => ModalInstance == null;
        private IModalReference modalReference;
        public IModalReference Show(string title = null, ModalOptions options = null)
        {
            if (!IsInline)
                throw new InvalidOperationException("You can only show an inlined dialog.");
            if (modalReference != null)
                Close();
            var parameters = new ModalParameters()
            {
                [nameof(Class)] = Class,
                [nameof(Style)] = Style,
                [nameof(Tag)] = Tag,
                [nameof(TitleContent)] = TitleContent,
                [nameof(ModalContent)] = ModalContent,
                [nameof(ContentStyle)] = ContentStyle,
            };
            modalReference = ModalService.Show<RtrwModal>(title, parameters, options ?? Options);
            modalReference.Result.ContinueWith(t =>
            {
                isVisible = false;
                InvokeAsync(() => IsVisibleChanged.InvokeAsync(false));
            });
            return modalReference;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (IsInline)
            {
                if (isVisible && modalReference == null)
                    Show();
                else if (modalReference != null)
                {
                    if (IsVisible)
                        (modalReference.Modal as RtrwModal)?.ForceUpdate();
                    else
                        Close();
                }
            }
            base.OnAfterRender(firstRender);
        }
        internal void ForceUpdate() => StateHasChanged();
        public void Close(ModalResult result = null)
        {
            if (!IsInline || modalReference == null)
                return;
            modalReference.Close(result);
            modalReference = null;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ModalInstance?.Register(this);
        }
    }
}
