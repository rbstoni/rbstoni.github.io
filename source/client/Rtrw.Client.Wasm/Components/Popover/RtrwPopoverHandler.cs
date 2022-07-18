using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rtrw.Client.Wasm.Components.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Components
{
    public class RtrwPopoverHandler
    {
        private readonly IJSRuntime _runtime;
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private readonly Action _updater;
        private bool _detached;

        public RtrwPopoverHandler(RenderFragment fragment, IJSRuntime jsInterop, Action updater)
        {
            Fragment = fragment ?? throw new ArgumentNullException(nameof(fragment));
            _runtime = jsInterop ?? throw new ArgumentNullException(nameof(jsInterop));
            _updater = updater ?? throw new ArgumentNullException(nameof(updater));
            Id = Guid.NewGuid();
        }

        public DateTime? ActivationDate { get; private set; }
        public string? Class { get; private set; }
        public RtrwRender? ElementReference { get; set; }
        public RenderFragment? Fragment { get; private set; }
        public Guid Id { get; }
        public bool IsConnected { get; private set; }
        public bool ShowContent { get; private set; }
        public string? Style { get; private set; }
        public object? Tag { get; private set; }
        public Dictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();

        public async Task Detach()
        {
            await _semaphore.WaitAsync();
            try
            {
                _detached = true;

                if (IsConnected)
                {
                    await _runtime.InvokeVoidAsync("rtrwPopover.disconnect", Id);
                }
            }
            catch (JSDisconnectedException) { }
            catch (TaskCanceledException) { }
            finally
            {
                IsConnected = false;
                _semaphore.Release();
            }
        }
        public async Task Initialize()
        {
            await _semaphore.WaitAsync();
            try
            {
                if (_detached)
                {
                    // If _detached is True, it means Detach() was invoked before Initialize() has had
                    // a chance to run. In this case, we just want to return and not do anything else
                    // otherwise we will end up with a memory leak.
                    return;
                }

                await _runtime.InvokeVoidAsync("rtrwPopover.connect", Id);
                IsConnected = true;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        public void SetComponentBaseParameters(RtrwComponentBase componentBase, string @class, string style, bool showContent)
        {
            Class = @class;
            Style = style;
            Tag = componentBase.Tag;
            Attributes = componentBase.UserAttributes;
            ShowContent = showContent;
            if (showContent == true)
            {
                ActivationDate = DateTime.Now;
            }
            else
            {
                ActivationDate = null;
            }
        }
        public void UpdateFragment(RenderFragment fragment,
            RtrwComponentBase componentBase, string @class, string style, bool showContent)
        {
            Fragment = fragment;
            SetComponentBaseParameters(componentBase, @class, @style, showContent);
            // this basically calls StateHasChanged on the Popover
            ElementReference?.ForceRender();
            _updater?.Invoke(); // <-- this doesn't do anything anymore except making unit tests happy 
        }
    }
}
