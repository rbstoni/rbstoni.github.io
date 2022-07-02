using Microsoft.AspNetCore.Components;
using Rtrw.Client.Wasm.Components;
using Rtrw.Client.Wasm.Models;
using Rtrw.Client.Wasm.Pages.Posting.Components;

namespace Rtrw.Client.Wasm.Pages.Posting
{
    public partial class RtrwNewPost
    {
        [Parameter] public Post Post { get; set; } = new();
        [Inject] IModalService ModalService { get; set; }
        void OpenChangePostCategoryModal()
        {
            ModalOptions options = new()
            {
                FullScreen = false,
                CloseButton = true,
                NoHeader = true
            };
            var modal = ModalService.Show<RtrwChangePostCategoryModal>(String.Empty, options);
            var result = modal.Result;
        }
        private void HandleValidSubmit()
        {

        }
    }
}
