using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Rtrw.Client.Wasm.Components.Enums;
using System.Collections.ObjectModel;

namespace Rtrw.Client.Wasm.Components
{
    public partial class RtrwModalProvider : IDisposable
    {

        private readonly ModalOptions globalModalOptions = new();
        private readonly Collection<IModalReference> modalRefs = new();

        [Parameter] public bool? CloseButton { get; set; }
        [Parameter] public bool? DisableBackdropClick { get; set; }
        [Parameter] public bool? NoHeader { get; set; } = true;
        [Parameter] public ModalPosition? Position { get; set; }
        [Inject] private IModalService? ModalService { get; set; }
        [Inject] private NavigationManager? NavigationManager { get; set; }

        public void DismissAll()
        {
            modalRefs.ToList().ForEach(rf => DismissInstance(rf, ModalResult.Cancel()));
            StateHasChanged();
        }
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
        internal void DismissInstance(Guid id, ModalResult result)
        {
            var reference = GetModalReference(id);
            if (reference != null)
                DismissInstance(reference, result);
        }
        protected override void OnInitialized()
        {
            ModalService!.OnModalInstanceAdded += AddInstance;
            ModalService.OnModalCloseRequested += DismissInstance;
            NavigationManager!.LocationChanged += LocationChanged;

            globalModalOptions.DisableBackdropClick = DisableBackdropClick;
            globalModalOptions.CloseButton = CloseButton;
            globalModalOptions.NoHeader = NoHeader;
            globalModalOptions.Position = Position;
        }
        private void AddInstance(IModalReference modalRef)
        {
            modalRefs.Add(modalRef);
            StateHasChanged();
        }
        private void DismissInstance(IModalReference modalRef, ModalResult result)
        {
            if (!modalRef.Dismiss(result)) return;

            modalRefs.Remove(modalRef);
            StateHasChanged();
        }
        private IModalReference GetModalReference(Guid id)
        {
            var rf = modalRefs.SingleOrDefault(x => x.Id == id);
            if (rf != null) return rf;
            return null!;
        }
        private void LocationChanged(object sender, LocationChangedEventArgs args) => DismissAll();

    }
}