namespace Rtrw.Client.Wasm.Components.Interfaces
{
    public interface IMask
    {
        string Mask { get; }
        string Text { get; }
        string GetCleanText() => Text;
        int CaretPos { get; set; }
        (int, int)? Selection { get; set; }
        void Insert(string input);
        void Delete();
        void Backspace();
        void Clear();
        void SetText(string text);
        void UpdateFrom(IMask other);
    }
}
