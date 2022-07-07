namespace Rtrw.Client.Wasm.Components.Mask
{
    public struct MaskChar
    {

        public MaskChar(char c, string regex)
        {
            Char = c;
            Regex = regex;
        }

        public char Char { get; set; }
        public string Regex { get; set; }

        public static MaskChar Digit(char c) => new MaskChar { Char = c, Regex = @"\d" };
        public static MaskChar Letter(char c) => new MaskChar { Char = c, Regex = @"\p{L}" };
        public static MaskChar LetterOrDigit(char c) => new MaskChar { Char = c, Regex = @"\p{L}|\d" };

    }
}
