namespace Rtrw.Client.Wasm.Models
{
    public class PollAnswer
    {
        public string Id { get; set; } = $"pollAnswer-{Guid.NewGuid().ToString("N")}";
        public string Answer { get; set; }
        public List<Warga>? PickedBy { get; set; } = default!;
        public long? PickedCount => PickedBy?.Count;
    }
}
