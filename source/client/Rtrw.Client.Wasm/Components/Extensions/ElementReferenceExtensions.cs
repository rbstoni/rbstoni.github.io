using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rtrw.Client.Wasm.Components.Interop;
using System.Reflection;

namespace Rtrw.Client.Wasm.Components.Extensions
{
    public static class ElementReferenceExtensions
    {
        private static readonly PropertyInfo jsRuntimeProperty =
            typeof(WebElementReferenceContext).GetProperty("JSRuntime", BindingFlags.Instance | BindingFlags.NonPublic);

        internal static IJSRuntime GetJSRuntime(this ElementReference elementReference)
        {
            if (elementReference.Context is not WebElementReferenceContext context)
            {
                return null;
            }

            return (IJSRuntime)jsRuntimeProperty.GetValue(context);
        }

        public static ValueTask<BoundingClientRect> MudGetBoundingClientRectAsync(this ElementReference elementReference)
        {
            return elementReference
                .GetJSRuntime()?
                .InvokeAsync<BoundingClientRect>("rtrwElementRef.getBoundingClientRect", elementReference) ?? ValueTask.FromResult(new BoundingClientRect());
        }

        public static ValueTask<int> RtrwAddEventListenerAsync<T>(this ElementReference elementReference, DotNetObjectReference<T> dotnet, string @event, string callback, bool stopPropagation = false) where T : class
        {
            var parameters = dotnet?.Value.GetType().GetMethods().First(m => m.Name == callback).GetParameters().Select(p => p.ParameterType);
            if (parameters != null)
            {
                var parameterSpecs = new object[parameters.Count()];
                for (var i = 0; i < parameters.Count(); ++i)
                {
                    parameterSpecs[i] = GetSerializationSpec(parameters.ElementAt(i));
                }
                return elementReference.GetJSRuntime()?.InvokeAsync<int>("rtrwElementRef.addEventListener", elementReference, dotnet, @event, callback, parameterSpecs, stopPropagation) ?? ValueTask.FromResult(0);
            }
            else
            {
                return new ValueTask<int>(0);
            }
        }

        public static ValueTask RtrwRemoveEventListenerAsync(this ElementReference elementReference, string @event, int eventId)
        {
            return elementReference
                .GetJSRuntime()?
                .InvokeVoidAsync("rtrwElementRef.removeEventListener", elementReference, eventId) ?? ValueTask.CompletedTask;
        }
        private static object GetSerializationSpec(Type type)
        {
            var props = type.GetProperties();
            var propsSpec = new Dictionary<string, object>();
            foreach (var prop in props)
            {
                if (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(string))
                {
                    propsSpec.Add(prop.Name.ToJsString(), "*");
                }
                else if (prop.PropertyType.IsArray)
                {
                    propsSpec.Add(prop.Name.ToJsString(), GetSerializationSpec(prop.PropertyType.GetElementType()));
                }
                else if (prop.PropertyType.IsClass)
                {
                    propsSpec.Add(prop.Name.ToJsString(), GetSerializationSpec(prop.PropertyType));
                }
            }
            return propsSpec;
        }
    }
}
