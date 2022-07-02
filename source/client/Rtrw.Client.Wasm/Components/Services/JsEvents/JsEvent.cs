using Microsoft.JSInterop;
using Rtrw.Client.Wasm.Components.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Rtrw.Client.Wasm.Components.Services.JsEvents
{
    public interface IJsEvent : IDisposable
    {
        event Action<int> CaretPositionChanged;
        event Action<string> Paste;
        event Action<int, int> Select;

        Task Connect(string elementId, JsEventOptions options);

        Task Disconnect();
    }

    public class JsEvent : IJsEvent
    {
        internal HashSet<string> _subscribedEvents = new HashSet<string>();
        private readonly DotNetObjectReference<JsEvent> _dotNetRef;
        private readonly IJSRuntime _jsRuntime;
        List<Action<int>> _caretPositionChangedHandlers = new List<Action<int>>();
        private string _elementId;
        private bool _isDisposed = false;
        private bool _isObserving;
        List<Action<string>> _pasteHandlers = new List<Action<string>>();
        List<Action<int, int>> _selectHandlers = new List<Action<int, int>>();

        [DynamicDependency(nameof(OnCaretPositionChanged))]
        [DynamicDependency(nameof(OnPaste))]
        [DynamicDependency(nameof(OnSelect))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JsEventOptions))]
        public JsEvent(IJSRuntime jsRuntime)
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            _jsRuntime = jsRuntime;
        }

        public event Action<int> CaretPositionChanged
        {
            add
            {
                if (_caretPositionChangedHandlers.Count == 0)
                {
                    Subscribe("click");
                    Subscribe("keyup");
                }
                _caretPositionChangedHandlers.Add(value);
            }
            remove
            {
                if (_caretPositionChangedHandlers.Count == 0)
                    return;
                if (_caretPositionChangedHandlers.Count == 1)
                {
                    Unsubscribe("click").AndForget();
                    Unsubscribe("keyup").AndForget();
                }
                _caretPositionChangedHandlers.Remove(value);
            }
        }
        public event Action<string> Paste
        {
            add
            {
                if (_pasteHandlers.Count == 0)
                    Subscribe("paste");
                _pasteHandlers.Add(value);
            }
            remove
            {
                if (_pasteHandlers.Count == 0)
                    return;
                if (_pasteHandlers.Count == 1)
                    Unsubscribe("paste").AndForget();
                _pasteHandlers.Remove(value);
            }
        }
        public event Action<int, int> Select
        {
            add
            {
                if (_selectHandlers.Count == 0)
                    Subscribe("select");
                _selectHandlers.Add(value);
            }
            remove
            {
                if (_selectHandlers.Count == 0)
                    return;
                if (_selectHandlers.Count == 1)
                    Unsubscribe("select").AndForget();
                _selectHandlers.Remove(value);
            }
        }

        public async Task Connect(string elementId, JsEventOptions options)
        {
            if (_isObserving)
                return;
            _elementId = elementId;
            try
            {
                await _jsRuntime.InvokeVoidAsync("mudJsEvent.connect", _dotNetRef, elementId, options);
                _isObserving = true;
            }
            catch (JSDisconnectedException)
            {
            }
            catch (TaskCanceledException)
            {
            }
        }

        public async Task Disconnect()
        {
            if (_elementId == null)
                return;
            await UnsubscribeAll();
            try
            {
                await _jsRuntime.InvokeVoidAsync($"mudJsEvent.disconnect", _elementId);
            }
            catch (Exception)
            {  /*ignore*/
            }
            _isObserving = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [JSInvokable]
        public void OnCaretPositionChanged(int caretPosition)
        {
            foreach (var handler in _caretPositionChangedHandlers)
            {
                handler.Invoke(caretPosition);
            }
        }

        [JSInvokable]
        public void OnPaste(string text)
        {
            foreach (var handler in _pasteHandlers)
            {
                handler.Invoke(text);
            }
        }

        [JSInvokable]
        public void OnSelect(int start, int end)
        {
            foreach (var handler in _selectHandlers)
            {
                handler.Invoke(start, end);
            }
        }

        internal void Subscribe(string eventName)
        {
            if (_elementId == null)
                throw new InvalidOperationException("Call Connect(...) before attaching events!");
            if (_subscribedEvents.Contains(eventName))
                return;
            try
            {
                _jsRuntime.InvokeVoidAsync("mudJsEvent.subscribe", _elementId, eventName).AndForget();
                _subscribedEvents.Add(eventName);
            }
            catch (JSDisconnectedException)
            {
            }
            catch (TaskCanceledException)
            {
            }
        }

        internal async Task Unsubscribe(string eventName)
        {
            if (_elementId == null)
                return;
            try
            {
                await _jsRuntime.InvokeVoidAsync($"mudJsEvent.unsubscribe", _elementId, eventName);
            }
            catch (Exception)
            {  /*ignore*/
            }
            _subscribedEvents.Remove(eventName);
        }

        internal async Task UnsubscribeAll()
        {
            if (_elementId == null)
                return;
            try
            {
                foreach (var eventName in _subscribedEvents)
                    await _jsRuntime.InvokeVoidAsync($"mudJsEvent.unsubscribe", _elementId, eventName);
            }
            catch (Exception)
            {  /*ignore*/
            }
            _subscribedEvents.Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _isDisposed)
                return;
            _isDisposed = true;
            Disconnect().AndForget();
            _dotNetRef.Dispose();
        }
    }

    public class JsEventOptions
    {
        public bool EnableLogging { get; set; } = false;
        public string TagName { get; set; }
        public string TargetClass { get; set; }
    }
}
