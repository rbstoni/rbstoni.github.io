using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.JsInterop
{
    public interface IMapboxJsInterop
    {
        ValueTask<IJSObjectReference> LoadMapToElement(ElementReference element);
        Task SetMapCenterAsync(IJSObjectReference mapInstance, double latitude, double longitude);

    }
    public class MapboxJsInterop : IMapboxJsInterop, IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        public MapboxJsInterop(IJSRuntime jSRuntime)
        {
            moduleTask = new(() => jSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/mapboxElement.js").AsTask());
        }

        public async ValueTask<IJSObjectReference> LoadMapToElement(ElementReference element)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<IJSObjectReference>("loadMapToElement", element).AsTask();
        }

        public async Task SetMapCenterAsync(IJSObjectReference mapInstance, double longitude, double latitude)
        {
            var module = await moduleTask.Value;
            if (module is not null && mapInstance is not null)
            {
                await module.InvokeVoidAsync("setMapCenter", mapInstance, longitude, latitude).AsTask();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}
