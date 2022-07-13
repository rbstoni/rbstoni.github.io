using System.Collections;

namespace Rtrw.Client.Wasm.Components
{
    public class ModalParameters : IEnumerable<KeyValuePair<string, object>>
    {

        internal Dictionary<string, object> modalParameters;

        public ModalParameters() => modalParameters = new Dictionary<string, object>();

        public int Count => modalParameters.Count;

        public object this[string parameterName]
        {
            get => Get<object>(parameterName);
            set => modalParameters[parameterName] = value;
        }

        IEnumerator IEnumerable.GetEnumerator() => modalParameters.GetEnumerator();

        public void Add(string parameterName, object value) => modalParameters[parameterName] = value;
        public T Get<T>(string parameterName)
        {
            if (modalParameters.TryGetValue(parameterName, out var value))
                return (T)value;

            throw new KeyNotFoundException($"{parameterName} does not exist in Dialog parameters");
        }
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => modalParameters.GetEnumerator();
        public T TryGet<T>(string parameterName)
        {
            if (modalParameters.TryGetValue(parameterName, out var value))
                return (T)value;

            return default!;
        }

    }
}
