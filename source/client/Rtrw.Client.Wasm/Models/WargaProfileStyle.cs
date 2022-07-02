using Rtrw.Client.Wasm.Helpers;

namespace Rtrw.Client.Wasm.Models
{
    public class WargaProfileStyle
    {
        public string Id { get; set; } = $"style-{Guid.NewGuid().ToString("N")}";
        public string BackgroundColor => Randomizer.GenerateRandomHexColor();
        public string ForegroundColor => Randomizer.GenerateRandomHexColor();
    }
}
