using System.Runtime.InteropServices;

namespace Rtrw.Client.Wasm.Components.Utilities
{
    public class RuntimeLocation
    {
        public static bool IsClientSide => RuntimeInformation.OSDescription == "Browser"; // WASM
        public static bool IsServerSide => RuntimeInformation.OSDescription != "Browser";
    }
}
