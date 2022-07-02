namespace Rtrw.Client.Wasm.Components
{
    public class ModalResult
    {

        protected internal ModalResult(object data, Type resultType, bool cancelled)
        {
            Data = data;
            DataType = resultType;
            Cancelled = cancelled;
        }

        public bool Cancelled { get; }
        public object Data { get; }
        public Type DataType { get; }

        public static ModalResult Cancel() => new(default, typeof(object), true);
        public static ModalResult Ok<T>(T result) => Ok(result, default);
        public static ModalResult Ok<T>(T result, Type modalType) => new(result, modalType, false);

    }
}
