using Microsoft.AspNetCore.Components;
using System.Diagnostics;

namespace Rtrw.Client.Wasm.Components
{
    public interface IModalReference
    {

        Guid Id { get; }
        RenderFragment RenderFragment { get; set; }

        bool AreParametersRendered { get; set; }

        Task<ModalResult> Result { get; }

        void Close();
        void Close(ModalResult result);

        bool Dismiss(ModalResult result);

        object Modal { get; }

        void InjectRenderFragment(RenderFragment rf);

        void InjectModal(object inst);

        Task<T> GetReturnValueAsync<T>();
    }

    public class ModalReference : IModalReference
    {

        private readonly IModalService modalService;
        private readonly TaskCompletionSource<ModalResult> resultCompletion = new();

        public ModalReference(Guid modalInstanceId, IModalService modalService)
        {
            Id = modalInstanceId;
            this.modalService = modalService;
        }

        public bool AreParametersRendered { get; set; }
        public Guid Id { get; }
        public object Modal { get; private set; }
        public RenderFragment RenderFragment { get; set; }
        public Task<ModalResult> Result => resultCompletion.Task;

        public void Close()
        {
            modalService.Close(this);
        }
        public void Close(ModalResult result)
        {
            modalService.Close(this, result);
        }
        public virtual bool Dismiss(ModalResult result)
        {
            return resultCompletion.TrySetResult(result);
        }
        public async Task<T> GetReturnValueAsync<T>()
        {
            var result = await Result;
            try
            {
                return (T)result.Data;
            }
            catch (InvalidCastException)
            {
                Debug.WriteLine($"Could not cast return value to {typeof(T)}, returning default.");
                return default;
            }
        }
        public void InjectModal(object inst)
        {
            Modal = inst;
        }
        public void InjectRenderFragment(RenderFragment rf)
        {
            RenderFragment = rf;
        }

    }
}
