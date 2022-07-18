using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Rtrw.Client.Wasm.Components.Extensions;

namespace Rtrw.Client.Wasm.Components.Services
{
    public delegate void KeyboardEvent(KeyboardEventArgs args);

    public interface IKeyInterceptor : IDisposable
    {
        Task Connect(string elementId, KeyInterceptorOptions options);
        Task Disconnect();
        Task UpdateKey(KeyOptions option);

        event KeyboardEvent KeyDown;
        event KeyboardEvent KeyUp;

    }
    public class KeyInterceptor : IKeyInterceptor, IDisposable
    {
        private bool _isDisposed = false;

        private readonly DotNetObjectReference<KeyInterceptor> _dotNetRef;
        private readonly IJSRuntime _jsRuntime;
        private bool _isObserving;
        private string _elementId;

        public KeyInterceptor(IJSRuntime jsRuntime)
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            _jsRuntime = jsRuntime;
        }
        public async Task Connect(string elementId, KeyInterceptorOptions options)
        {
            if (_isObserving)
                return;
            _elementId = elementId;
            try
            {
                await _jsRuntime.InvokeVoidAsync("rtrwKeyInterceptor.connect", _dotNetRef, elementId, options);
                _isObserving = true;
            }
            catch (JSDisconnectedException) { }
            catch (TaskCanceledException) { }
        }
        public async Task UpdateKey(KeyOptions option)
        {
            await _jsRuntime.InvokeVoidAsync($"rtrwKeyInterceptor.updatekey", _elementId, option);
        }
        public async Task Disconnect()
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync($"rtrwKeyInterceptor.disconnect", _elementId);
            }
            catch (Exception) {  /*ignore*/ }
            _isObserving = false;
        }

        [JSInvokable]
        public void OnKeyDown(KeyboardEventArgs args)
        {
            KeyDown?.Invoke(args);
        }

        [JSInvokable]
        public void OnKeyUp(KeyboardEventArgs args)
        {
            KeyUp?.Invoke(args);
        }

        public event KeyboardEvent KeyDown;
        public event KeyboardEvent KeyUp;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _isDisposed)
                return;
            _isDisposed = true;
            Disconnect().AndForget();
            _dotNetRef.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
