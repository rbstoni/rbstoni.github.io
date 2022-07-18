namespace Rtrw.Client.Wasm.Models
{
    public class Poll
    {
        public string Id { get; set; } = $"poll-{Guid.NewGuid().ToString("N")}";
        public string Question { get; set; }
        public List<Report>? Reports { get; set; }
        public List<PollAnswer> Answers { get; set; } = new();
    }
}
