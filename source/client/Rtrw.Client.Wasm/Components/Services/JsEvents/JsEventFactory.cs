using Microsoft.JSInterop;

namespace Rtrw.Client.Wasm.Components.Services.JsEvents
{
    public interface IJsEventFactory
    {
        IJsEvent Create();
    }

    public class JsEventFactory : IJsEventFactory
    {
        private readonly IServiceProvider _provider;

        public JsEventFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IJsEvent Create() =>
            new JsEvent(_provider.GetRequiredService<IJSRuntime>());
    }
}
