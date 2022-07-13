using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Rtrw.Client.Wasm.FakeData.JSInterop
{
    public interface IBrowserCache
    {
        Task<int> SyncDbWithCacheAsync(string filename);

        Task<bool> GenerateDownloadLinkAsync(ElementReference parent, string filename);
    }

    public class BrowserCache : IBrowserCache
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        public BrowserCache(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/browserCache.js").AsTask()!);
        }

        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }

        public async Task<int> SyncDbWithCacheAsync(string filename)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<int>("synchronizeDbWithCache", filename);
        }

        public async Task<bool> GenerateDownloadLinkAsync(ElementReference parent, string filename)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<bool>("generateDownloadLink", parent, filename);
        }
    }
}
