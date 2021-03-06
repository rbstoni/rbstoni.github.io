using Microsoft.AspNetCore.Components;
using Rtrw.Client.Wasm.Components.Base;
using Rtrw.Client.Wasm.Utilities;

namespace Rtrw.Client.Wasm.Components
{
    public partial class RtrwModalInstance : RtrwComponentBase, IDisposable
    {

        private bool disposedValue;
        private string elementId = "modal-" + Guid.NewGuid().ToString().Substring(0, 8);
        private ModalOptions modalOptions = new();
        private RtrwModal? rtrwModal;

        [Parameter] public RenderFragment? Content { get; set; }
        [Parameter] public Guid Id { get; set; }
        [Parameter]
        public ModalOptions Options
        {
            get
            {
                if (modalOptions == null)
                    modalOptions = new ModalOptions();
                return modalOptions;
            }
            set => modalOptions = value;
        }
        [Parameter] public string? Title { get; set; }
        [Parameter] public RenderFragment? TitleContent { get; set; }
        protected string Classname
            => new CssBuilder("rtrw-modal")
                .AddClass("rtrw-modal-fullscreen", FullScreen)
                .AddClass(Class)
                .Build();
        private bool CloseButton { get; set; }
        private bool DisableBackdropClick { get; set; }
        private bool FullScreen { get; set; }
        [CascadingParameter] private ModalOptions GlobalModalOptions { get; set; } = new ModalOptions();
        private bool NoHeader { get; set; }
        [CascadingParameter] private RtrwModalProvider? Parent { get; set; }

        public void Cancel() => Close(ModalResult.Cancel());
        public void Close() => Close(ModalResult.Ok<object>(null));
        public void Close(ModalResult modalResult) => Parent?.DismissInstance(Id, modalResult);
        public void Close<T>(T returnValue)
        {
            var modalResult = ModalResult.Ok<T>(returnValue);
            Parent?.DismissInstance(Id, modalResult);
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        public void ForceRender() => StateHasChanged();
        public void Register(RtrwModal modal)
        {
            if (modal == null)
                return;
            rtrwModal = modal;
            Class = modal.Class;
            Style = modal.Style;
            TitleContent = modal.TitleContent;
            StateHasChanged();
        }
        public void SetOptions(ModalOptions options)
        {
            Options = options;
            ConfigureInstance();
            StateHasChanged();
        }
        public void SetTitle(string title)
        {
            Title = title;
            StateHasChanged();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                disposedValue = true;
            }
        }
        protected override void OnInitialized() => ConfigureInstance();
        private void ConfigureInstance()
        {
            Class = Classname;
            NoHeader = SetHideHeader();
            CloseButton = SetCloseButton();
            FullScreen = SetFulScreen();
            DisableBackdropClick = SetDisableBackdropClick();
        }
        private void HandleBackgroundClick()
        {
            if (DisableBackdropClick)
                return;

            if (rtrwModal?.OnBackdropClick == null)
            {
                Cancel();
                return;
            }

            rtrwModal?.OnBackdropClick.Invoke();
        }
        private bool SetCloseButton()
        {
            if (Options.CloseButton.HasValue)
                return Options.CloseButton.Value;

            if (GlobalModalOptions.CloseButton.HasValue)
                return GlobalModalOptions.CloseButton.Value;

            return false;
        }
        private bool SetDisableBackdropClick()
        {
            if (Options.DisableBackdropClick.HasValue)
                return Options.DisableBackdropClick.Value;

            if (GlobalModalOptions.DisableBackdropClick.HasValue)
                return GlobalModalOptions.DisableBackdropClick.Value;

            return false;
        }
        private bool SetFulScreen()
        {
            if (Options.FullScreen.HasValue)
                return Options.FullScreen.Value;

            if (GlobalModalOptions.FullScreen.HasValue)
                return GlobalModalOptions.FullScreen.Value;

            return false;
        }
        private bool SetHideHeader()
        {
            if (Options.NoHeader.HasValue)
                return Options.NoHeader.Value;

            if (GlobalModalOptions.NoHeader.HasValue)
                return GlobalModalOptions.NoHeader.Value;

            return false;
        }

    }
}
