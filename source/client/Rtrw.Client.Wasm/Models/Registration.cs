namespace Rtrw.Client.Wasm.Models
{
    public class Registration
    {
        public string Id { get; set; } = $"registration-{Guid.NewGuid().ToString("N")}";
        public int CurrentStep { get; set; }
        public int CurrentProgress { get; set; }
    }
}
