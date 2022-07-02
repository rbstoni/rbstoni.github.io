using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Rtrw.Client.Wasm.Pages.Peta
{
    public partial class RtrwPeta
    {
        private ElementReference mapElement;
        private IJSObjectReference? mapModule;
        private IJSObjectReference? mapInstance;
        private IJSObjectReference? mapControl;

        private bool open;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                mapModule = await JS.InvokeAsync<IJSObjectReference>(
                    "import", "./js/mapComponent.js");
                mapInstance = await mapModule.InvokeAsync<IJSObjectReference>(
                    "addMapToElement", mapElement);
                mapControl = await mapModule.InvokeAsync<IJSObjectReference>(
                    "addMapControl", mapInstance);
            }
        }

        private async Task ShowAsync(double latitude, double longitude)
        {
            if (mapModule is not null && mapInstance is not null)
            {
                await mapModule.InvokeVoidAsync("setMapCenter", mapInstance,
                    latitude, longitude).AsTask();
            }
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (mapInstance is not null)
            {
                await mapInstance.DisposeAsync();
            }

            if (mapModule is not null)
            {
                await mapModule.DisposeAsync();
            }
        }

        protected override void OnInitialized()
        {
            open = true;
            base.OnInitialized();
        }
    }
}
