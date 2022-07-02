using Microsoft.JSInterop;
using Rtrw.Client.Wasm.Components.Extensions;

namespace Rtrw.Client.Wasm.Components.Services.Scroll
{
    public class ScrollManager : IScrollManager
    {
        private readonly IJSRuntime jSRuntime;

        public ScrollManager(IJSRuntime jSRuntime)
        {
            this.jSRuntime = jSRuntime;
        }

        public ValueTask ScrollToFragmentAsync(string id, ScrollBehavior behavior) =>
            jSRuntime.InvokeVoidAsync("scrollManager.scrollToFragment", id, behavior.EnumToDescriptionString());

        public ValueTask ScrollToAsync(string id, int left, int top, ScrollBehavior behavior) =>
            jSRuntime.InvokeVoidAsync("scrollManager.scrollTo", id, left, top, behavior.EnumToDescriptionString());

        public ValueTask ScrollToTopAsync(string id, ScrollBehavior scrollBehavior = ScrollBehavior.Auto) =>
            ScrollToAsync(id, 0, 0, scrollBehavior);

        public ValueTask ScrollToBottomAsync(string id, ScrollBehavior behavior) =>
            jSRuntime.InvokeVoidAsync("scrollManager.scrollToBottom", id, behavior.EnumToDescriptionString());

        public ValueTask ScrollToYearAsync(string elementId) =>
            jSRuntime.InvokeVoidAsync("scrollManager.scrollToYear", elementId);

        public ValueTask ScrollToListItemAsync(string elementId) =>
            jSRuntime.InvokeVoidAsync("scrollManager.scrollToListItem", elementId);

        public ValueTask LockScrollAsync(string selector = "body", string cssClass = "scroll-locked") =>
            jSRuntime.InvokeVoidAsync("scrollManager.lockScroll", selector, cssClass);

        public ValueTask UnlockScrollAsync(string selector = "body", string cssClass = "scroll-locked") =>
            jSRuntime.InvokeVoidAsync("scrollManager.unlockScroll", selector, cssClass);
    }

    public interface IScrollManager
    {
        ValueTask ScrollToAsync(string id, int left, int top, ScrollBehavior scrollBehavior);
        ValueTask ScrollToFragmentAsync(string id, ScrollBehavior behavior);
        ValueTask ScrollToTopAsync(string id, ScrollBehavior scrollBehavior = ScrollBehavior.Auto);
        ValueTask ScrollToYearAsync(string elementId);
        ValueTask ScrollToListItemAsync(string elementId);
        ValueTask LockScrollAsync(string selector = "body", string cssClass = "scroll-locked");
        ValueTask UnlockScrollAsync(string selector = "body", string cssClass = "scroll-locked");
        ValueTask ScrollToBottomAsync(string elementId, ScrollBehavior scrollBehavior = ScrollBehavior.Auto);
    }

    public enum ScrollBehavior
    {
        Smooth,
        Auto
    }
}
