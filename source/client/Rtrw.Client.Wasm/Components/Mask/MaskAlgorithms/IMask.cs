namespace Rtrw.Client.Wasm.Components.Mask.MaskAlgorithms
{
    public interface IMask
    {
        int CaretPos { get; set; }
        string Mask { get; }
        (int, int)? Selection { get; set; }
        string Text { get; }

        void Backspace();
        void Clear();
        void Delete();
        string GetCleanText() => Text;
        void Insert(string input);
        void SetText(string text);
        void UpdateFrom(IMask other);
    }
}
