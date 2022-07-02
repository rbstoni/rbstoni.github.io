using Microsoft.JSInterop;

namespace Rtrw.Client.Wasm.Services
{
    public interface IWindowHistoryService
    {
        ValueTask Back();
        ValueTask Forward();
    }
    public class WindowHistoryService : IWindowHistoryService
    {
        private readonly IJSRuntime jSRuntime;

        public WindowHistoryService(IJSRuntime jSRuntime) => this.jSRuntime = jSRuntime;
        public ValueTask Back() => jSRuntime.InvokeVoidAsync("window.history.back");

        public ValueTask Forward() => jSRuntime.InvokeVoidAsync("window.history.forward");
    }
}
