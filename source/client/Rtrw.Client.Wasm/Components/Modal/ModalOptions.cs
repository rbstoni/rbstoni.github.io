using Rtrw.Client.Wasm.Components.Enums;

namespace Rtrw.Client.Wasm.Components
{
    public class ModalOptions
    {
        public bool? DisableBackdropClick { get; set; }
        public bool? CloseButton { get; set; }
        public bool? NoHeader { get; set; }
        public bool? FullScreen { get; set; }
        public ModalPosition? Position { get; set; }
        public bool? FullWidth { get; set; }
    }
}
