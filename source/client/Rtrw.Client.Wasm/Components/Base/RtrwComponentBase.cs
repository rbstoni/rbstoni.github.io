using Microsoft.AspNetCore.Components;

namespace Rtrw.Client.Wasm.Components.Base
{
    public abstract class RtrwComponentBase : ComponentBase, IHandleEvent
    {

        [Parameter] public string? Class { get; set; }
        public string FieldId => UserAttributes?.ContainsKey("id") == true ? UserAttributes["id"].ToString() : $"rtrw-input-{Guid.NewGuid()}";
        [Parameter] public string? Style { get; set; }
        [Parameter] public object? Tag { get; set; }
        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> UserAttributes { get; set; } = new Dictionary<string, object>();
        protected bool IsNotRender { get; set; }

        Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem eventCallback, object? arg)
        {
            var task = eventCallback.InvokeAsync(arg);
            var shouldAwaitTask = task.Status != TaskStatus.RanToCompletion && task.Status != TaskStatus.Canceled;
            if (!IsNotRender)
            {
                // After each event, we synchronously re-render (unless !ShouldRender())
                // This just saves the developer the trouble of putting "StateHasChanged();"
                // at the end of every event callback.
                StateHasChanged();
            }
            else
            {
                IsNotRender = false;
            }

            return shouldAwaitTask ? CallStateHasChangedOnAsyncCompletion(task) : Task.CompletedTask;
        }

        async Task CallStateHasChangedOnAsyncCompletion(Task task)
        {
            try
            {
                await task;
            }
            catch
            {
                if (task.IsCanceled)
                    return;
                else
                    throw;
            }
            if (!IsNotRender)
                StateHasChanged();
            else
                IsNotRender = false;
        }

    }
}
