
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Rtrw.Client.Wasm.Components.Base;

namespace Rtrw.Client.Wasm.Components
{
    public interface IRtrwPopoverService
    {

        event EventHandler FragmentsChanged;

        IEnumerable<RtrwPopoverHandler> Handlers { get; }

        Task InitializeIfNeeded();
        RtrwPopoverHandler Register(RenderFragment fragment);
        Task<bool> Unregister(RtrwPopoverHandler hanlder);

    }

    public class RtrwPopoverService : IRtrwPopoverService, IAsyncDisposable
    {

        private readonly IJSRuntime _jsRuntime;
        private readonly PopoverOptions _options;
        private Dictionary<Guid, RtrwPopoverHandler> _handlers = new();
        private bool _isInitilized = false;
        private SemaphoreSlim _semaphoreSlim = new(1, 1);

        public RtrwPopoverService(IJSRuntime jsInterop, IOptions<PopoverOptions>? options = null)
        {
            _options = options?.Value ?? new PopoverOptions();
            _jsRuntime = jsInterop ?? throw new ArgumentNullException(nameof(jsInterop));
        }

        public event EventHandler FragmentsChanged;

        public IEnumerable<RtrwPopoverHandler> Handlers => _handlers.Values.AsEnumerable();

        [ExcludeFromCodeCoverage]
        public async ValueTask DisposeAsync()
        {
            if (_isInitilized == false) { return; }

            try
            {
                await _jsRuntime.InvokeVoidAsync("rtrwPopover.dispose");
            }
            catch (JSDisconnectedException) { }
            catch (TaskCanceledException) { }
        }
        public async Task InitializeIfNeeded()
        {
            if (_isInitilized == true) { return; }

            try
            {
                await _semaphoreSlim.WaitAsync();
                if (_isInitilized == true) { return; }

                await _jsRuntime.InvokeVoidAsync("rtrwPopover.initialize", _options.ContainerClass, _options.FlipMargin);
                _isInitilized = true;
            }
            catch (JSDisconnectedException) { }
            catch (TaskCanceledException) { }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
        public RtrwPopoverHandler Register(RenderFragment fragment)
        {
            var handler = new RtrwPopoverHandler(fragment, _jsRuntime, () => { /*not doing anything on purpose for now*/ });
            _handlers.Add(handler.Id, handler);

            FragmentsChanged?.Invoke(this, EventArgs.Empty);

            return handler;
        }
        public async Task<bool> Unregister(RtrwPopoverHandler handler)
        {
            if (handler == null) { return false; }
            if (_handlers.Remove(handler.Id) == false) { return false; }

            await handler.Detach();

            FragmentsChanged?.Invoke(this, EventArgs.Empty);

            return true;
        }

    }
}
