using Microsoft.AspNetCore.Components;

namespace Rtrw.Client.Wasm.Components
{
    public interface IModalService
    {

        public event Action<IModalReference, ModalResult> OnModalCloseRequested;
        public event Action<IModalReference> OnModalInstanceAdded;

        void Close(ModalReference modalReference);
        void Close(ModalReference modalReference, ModalResult modalResult);
        IModalReference CreateReference();
        IModalReference Show<TComponent>() where TComponent : ComponentBase;
        IModalReference Show<TComponent>(string title) where TComponent : ComponentBase;
        IModalReference Show(Type component);
        IModalReference Show<TComponent>(string title, ModalOptions options) where TComponent : ComponentBase;
        IModalReference Show<TComponent>(string title, ModalParameters parameters) where TComponent : ComponentBase;
        IModalReference Show(Type component, string title);
        IModalReference Show<TComponent>(string title, ModalParameters parameters = null, ModalOptions options = null) where TComponent : ComponentBase;
        IModalReference Show(Type component, string title, ModalOptions options);
        IModalReference Show(Type component, string title, ModalParameters parameters);
        IModalReference Show(Type component, string title, ModalParameters parameters, ModalOptions options);

    }

    public class ModalService : IModalService
    {

        public event Action<IModalReference, ModalResult> OnModalCloseRequested;
        public event Action<IModalReference> OnModalInstanceAdded;

        public void Close(ModalReference modalReference) => Close(modalReference, ModalResult.Ok<object>(null));
        public virtual void Close(ModalReference modalReference, ModalResult result) => OnModalCloseRequested?.Invoke(modalReference, result);
        public virtual IModalReference CreateReference() => new ModalReference(Guid.NewGuid(), this);
        public IModalReference Show<T>() where T : ComponentBase => Show<T>(string.Empty, new ModalParameters(), new ModalOptions());
        public IModalReference Show<T>(string title) where T : ComponentBase => Show<T>(title, new ModalParameters(), new ModalOptions());
        public IModalReference Show(Type contentComponent) => Show(contentComponent, string.Empty, new ModalParameters(), new ModalOptions());
        public IModalReference Show<T>(ModalOptions options) where T : ComponentBase => Show<T>(string.Empty, new ModalParameters(), options);
        public IModalReference Show<T>(ModalParameters parameters) where T : ComponentBase => Show<T>(string.Empty, parameters, new ModalOptions());
        public IModalReference Show<T>(string title, ModalOptions options) where T : ComponentBase => Show<T>(title, new ModalParameters(), options);
        public IModalReference Show<T>(string title, ModalParameters parameters) where T : ComponentBase => Show<T>(title, parameters, new ModalOptions());
        public IModalReference Show(Type contentComponent, string title) => Show(contentComponent, title, new ModalParameters(), new ModalOptions());
        public IModalReference Show(Type component, ModalOptions options) => Show(component, string.Empty, new ModalParameters(), options);
        public IModalReference Show(Type component, ModalParameters parameters) => Show(component, string.Empty, parameters, new ModalOptions());
        public IModalReference Show<T>(string title, ModalParameters parameters, ModalOptions options) where T : ComponentBase => Show(typeof(T), title, parameters, options);
        public IModalReference Show(Type contentComponent, string title, ModalOptions options) => Show(contentComponent, title, new ModalParameters(), options);
        public IModalReference Show(Type contentComponent, string title, ModalParameters parameters) => Show(contentComponent, title, parameters, new ModalOptions());
        public IModalReference Show(Type contentComponent, string title, ModalParameters parameters, ModalOptions options)
        {
            if (!typeof(ComponentBase).IsAssignableFrom(contentComponent))
            {
                throw new ArgumentException($"{contentComponent.FullName} must be a Blazor Component");
            }
            var modalReference = CreateReference();

            var modalContent = new RenderFragment(builder =>
            {
                var i = 0;
                builder.OpenComponent(i++, contentComponent);

                if (!modalReference.AreParametersRendered)
                {
                    foreach (var parameter in parameters)
                    {
                        builder.AddAttribute(i++, parameter.Key, parameter.Value);
                    }

                    modalReference.AreParametersRendered = true;
                }
                else
                {
                    i += parameters.Count;
                }

                builder.AddComponentReferenceCapture(i++, inst => { modalReference.InjectModal(inst); });
                builder.CloseComponent();
            });
            var modalInstance = new RenderFragment(builder =>
            {
                builder.OpenComponent<RtrwModalInstance>(0);
                builder.SetKey(modalReference.Id);
                builder.AddAttribute(1, "Options", options);
                builder.AddAttribute(2, "Title", title);
                builder.AddAttribute(3, "Content", modalContent);
                builder.AddAttribute(4, "Id", modalReference.Id);
                builder.CloseComponent();
            });
            modalReference.InjectRenderFragment(modalInstance);
            OnModalInstanceAdded?.Invoke(modalReference);

            return modalReference;
        }

    }
}
