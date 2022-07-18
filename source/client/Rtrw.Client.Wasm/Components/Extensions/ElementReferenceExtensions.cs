using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rtrw.Client.Wasm.Components.Interop;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Rtrw.Client.Wasm.Components.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ElementReferenceExtensions
    {
        private static readonly PropertyInfo jsRuntimeProperty = typeof(WebElementReferenceContext).GetProperty("JSRuntime", BindingFlags.Instance | BindingFlags.NonPublic);

        internal static IJSRuntime GetJSRuntime(this ElementReference elementReference)
        {
            if (elementReference.Context is not WebElementReferenceContext context)
            {
                return null!;
            }

            return (IJSRuntime)jsRuntimeProperty.GetValue(context);
        }

        public static ValueTask RtrwFocusFirstAsync(this ElementReference elementReference, int skip = 0, int min = 0) =>
            elementReference.GetJSRuntime()?.InvokeVoidAsync("rtrwElementRef.focusFirst", elementReference, skip, min) ?? ValueTask.CompletedTask;

        public static ValueTask RtrwFocusLastAsync(this ElementReference elementReference, int skip = 0, int min = 0) =>
            elementReference.GetJSRuntime()?.InvokeVoidAsync("rtrwElementRef.focusLast", elementReference, skip, min) ?? ValueTask.CompletedTask;

        public static ValueTask RtrwSaveFocusAsync(this ElementReference elementReference) =>
            elementReference.GetJSRuntime()?.InvokeVoidAsync("rtrwElementRef.saveFocus", elementReference) ?? ValueTask.CompletedTask;

        public static ValueTask RtrwRestoreFocusAsync(this ElementReference elementReference) =>
            elementReference.GetJSRuntime()?.InvokeVoidAsync("rtrwElementRef.restoreFocus", elementReference) ?? ValueTask.CompletedTask;

        public static ValueTask RtrwSelectAsync(this ElementReference elementReference) =>
            elementReference.GetJSRuntime()?.InvokeVoidAsync("rtrwElementRef.select", elementReference) ?? ValueTask.CompletedTask;

        public static ValueTask RtrwSelectRangeAsync(this ElementReference elementReference, int pos1, int pos2) =>
            elementReference.GetJSRuntime()?.InvokeVoidAsync("rtrwElementRef.selectRange", elementReference, pos1, pos2) ?? ValueTask.CompletedTask;

        public static ValueTask RtrwChangeCssAsync(this ElementReference elementReference, string css) =>
            elementReference.GetJSRuntime()?.InvokeVoidAsync("rtrwElementRef.changeCss", elementReference, css) ?? ValueTask.CompletedTask;

        public static ValueTask<BoundingClientRect> RtrwGetBoundingClientRectAsync(this ElementReference elementReference) =>
            elementReference.GetJSRuntime()?.InvokeAsync<BoundingClientRect>("rtrwElementRef.getBoundingClientRect", elementReference) ?? ValueTask.FromResult(new BoundingClientRect());

        /// <summary>
        /// Gets the client rect of the element 
        /// </summary>
        public static ValueTask<BoundingClientRect> RtrwGetClientRectFromParentAsync(this ElementReference elementReference) =>
           elementReference.GetJSRuntime()?.InvokeAsync<BoundingClientRect>("rtrwElementRef.getClientRectFromParent", elementReference) ?? ValueTask.FromResult(new BoundingClientRect());

        /// <summary>
        /// Gets the client rect of the first child of the element.
        /// Useful when you want to know the dimensions of a render fragment and for that you wrap it into a div
        /// </summary>
        public static ValueTask<BoundingClientRect> RtrwGetClientRectFromFirstChildAsync(this ElementReference elementReference) =>
           elementReference.GetJSRuntime()?.InvokeAsync<BoundingClientRect>("rtrwElementRef.getClientRectFromFirstChild", elementReference) ?? ValueTask.FromResult(new BoundingClientRect());

        /// <summary>
        /// Returns true if the element has an ancestor with style position == "fixed"
        /// </summary>
        /// <param name="elementReference"></param>
        public static ValueTask<bool> RtrwHasFixedAncestorsAsync(this ElementReference elementReference) =>
            elementReference.GetJSRuntime()?
            .InvokeAsync<bool>("rtrwElementRef.hasFixedAncestors", elementReference) ?? ValueTask.FromResult(false);


        public static ValueTask RtrwChangeCssVariableAsync(this ElementReference elementReference, string variableName, int value) =>
            elementReference.GetJSRuntime()?.InvokeVoidAsync("rtrwElementRef.changeCssVariable", elementReference, variableName, value) ?? ValueTask.CompletedTask;

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

        public static ValueTask RtrwRemoveEventListenerAsync(this ElementReference elementReference, string @event, int eventId) =>
            elementReference.GetJSRuntime()?.InvokeVoidAsync("rtrwElementRef.removeEventListener", elementReference, eventId) ?? ValueTask.CompletedTask;

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
