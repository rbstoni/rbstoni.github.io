using Rtrw.Client.Wasm.Models;

namespace Rtrw.Client.Wasm.FakeData.Services
{
    public interface ICurrentUser
    {
        Warga Warga { get; }
    }

    public class CurrentUser : ICurrentUser
    {
        private Warga warga = new Dummy().FakeWarga;
        public Warga Warga => warga;
    }
}
