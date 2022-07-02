using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using System.Collections.ObjectModel;

namespace Rtrw.Client.Wasm.Components
{
    public partial class RtrwModalProvider : IDisposable
    {
        [Inject] private IModalService ModalService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

        [Parameter] public bool? NoHeader { get; set; } = true;
        [Parameter] public bool? CloseButton { get; set; }
        [Parameter] public bool? DisableBackdropClick { get; set; }

        private readonly Collection<IModalReference> modalRefs = new();
        private readonly ModalOptions globalModalOptions = new();

        protected override void OnInitialized()
        {
            ModalService.OnModalInstanceAdded += AddInstance;
            ModalService.OnModalCloseRequested += DismissInstance;
            NavigationManager.LocationChanged += LocationChanged;

            globalModalOptions.DisableBackdropClick = DisableBackdropClick;
            globalModalOptions.CloseButton = CloseButton;
            globalModalOptions.NoHeader = NoHeader;
        }

        internal void DismissInstance(Guid id, ModalResult result)
        {
            var reference = GetModalReference(id);
            if (reference != null)
                DismissInstance(reference, result);
        }

        private void AddInstance(IModalReference modalRef)
        {
            modalRefs.Add(modalRef);
            StateHasChanged();
        }

        public void DismissAll()
        {
            modalRefs.ToList().ForEach(r => DismissInstance(r, ModalResult.Cancel()));
            StateHasChanged();
        }

        private void DismissInstance(IModalReference modalRef, ModalResult result)
        {
            if (!modalRef.Dismiss(result)) return;

            modalRefs.Remove(modalRef);
            StateHasChanged();
        }

        private IModalReference GetModalReference(Guid id) => modalRefs.SingleOrDefault(x => x.Id == id);

        private void LocationChanged(object sender, LocationChangedEventArgs args) => DismissAll();

        public void Dispose()
        {
            if (NavigationManager != null)
                NavigationManager.LocationChanged -= LocationChanged;

            if (ModalService != null)
            {
                ModalService.OnModalInstanceAdded -= AddInstance;
                ModalService.OnModalCloseRequested -= DismissInstance;
            }
        }
    }
}
