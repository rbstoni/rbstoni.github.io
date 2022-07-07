using Rtrw.Client.Wasm.Components.Interfaces;

namespace Rtrw.Client.Wasm.Components.Mask
{
    public abstract class BaseMask : IMask
    {

        protected HashSet<char> _delimiters;
        protected bool _initialized;
        protected MaskChar[] _maskChars = new MaskChar[]
        {
        MaskChar.Letter('a'), MaskChar.Digit('0'), MaskChar.LetterOrDigit('*'),
        };
        protected Dictionary<char, MaskChar> _maskDict;

        public bool AllowOnlyDelimiters { get; set; }
        public int CaretPos { get; set; }
        public string Mask { get; protected set; }
        public MaskChar[] MaskChars
        {
            get => _maskChars;
            set
            {
                _maskChars = value;
                // force re-initialization
                _initialized = false;
            }
        }
        public (int, int)? Selection { get; set; }
        public string Text { get; protected set; }

        public abstract void Backspace();
        public void Clear()
        {
            Init();
            Text = "";
            CaretPos = 0;
            Selection = null;
        }
        public abstract void Delete();
        public virtual string GetCleanText() => Text;
        public abstract void Insert(string input);
        public void SetText(string text)
        {
            Clear();
            Insert(text);
        }
        public override string ToString()
        {
            var text = Text ?? "";
            ConsolidateSelection();
            if (Selection == null)
            {
                var pos = ConsolidateCaret(text, CaretPos);
                if (pos < text.Length)
                    return text.Insert(pos, "|");
                return text + "|";
            }
            else
            {
                var sel = Selection.Value;
                var start = ConsolidateCaret(text, sel.Item1);
                var end = ConsolidateCaret(text, sel.Item2);
                (var s1, var rest) = SplitAt(text, start);
                (var s2, var s3) = SplitAt(rest, end - start);
                return s1 + "[" + s2 + "]" + s3;
            }
        }
        public virtual void UpdateFrom(IMask o)
        {
            var other = o as BaseMask;
            if (other == null)
                return;
            if (other.Mask != Mask)
            {
                Mask = other.Mask;
                _initialized = false;
            }
            if (other.MaskChars != null)
            {
                var maskChars = new HashSet<MaskChar>(_maskChars ?? new MaskChar[0]);
                if (other.MaskChars.Length != MaskChars.Length || other.MaskChars.Any(x => !maskChars.Contains(x)))
                {
                    _maskChars = other.MaskChars;
                    _initialized = false;
                }
            }
            Refresh();
        }
        internal static (string, string) SplitAt(string text, int pos)
        {
            if (pos <= 0)
                return ("", text);
            if (pos >= text.Length)
                return (text, "");
            return (text.Substring(0, pos), text.Substring(pos));
        }
        internal static (string, string, string) SplitSelection(string text, (int, int) selection)
        {
            var start = ConsolidateCaret(text, selection.Item1);
            var end = ConsolidateCaret(text, selection.Item2);
            (var s1, var rest) = SplitAt(text, start);
            (var s2, var s3) = SplitAt(rest, end - start);
            return (s1, s2, s3);
        }
        protected static int ConsolidateCaret(string text, int caretPos)
        {
            if (string.IsNullOrEmpty(text) || caretPos < 0)
                return 0;
            if (caretPos < text.Length)
                return caretPos;
            return text.Length;
        }
        protected void ConsolidateSelection()
        {
            if (Selection == null)
                return;
            var sel = Selection.Value;
            if (sel.Item1 == sel.Item2)
            {
                CaretPos = sel.Item1;
                Selection = null;
                return;
            }
            if (sel.Item1 < 0)
                sel.Item1 = 0;
            if (sel.Item2 >= Text.Length)
                sel.Item2 = Text.Length;
        }
        protected abstract void DeleteSelection(bool align);
        protected void Init()
        {
            if (_initialized)
                return;
            _initialized = true;
            InitInternals();
        }
        protected virtual void InitInternals()
        {
            _maskDict = _maskChars.ToDictionary(x => x.Char);
            if (Mask == null)
                _delimiters = new();
            else
                _delimiters =
                    new HashSet<char>(Mask.Where(c => _maskChars.All(maskDef => maskDef.Char != c)));
        }
        protected virtual bool IsDelimiter(char maskChar)
        {
            return _delimiters.Contains(maskChar);
        }
        protected virtual void Refresh()
        {
            var caret = CaretPos;
            var sel = Selection;
            SetText(Text);
            CaretPos = ConsolidateCaret(Text, caret);
            Selection = sel;
            if (sel != null)
                ConsolidateSelection();
        }
        protected virtual void UpdateText(string text)
        {
            // don't show a text consisting only of delimiters and placeholders (no actual input)
            if (!AllowOnlyDelimiters && text.All(c => _delimiters.Contains(c)))
            {
                Text = "";
                return;
            }
            Text = text;
            CaretPos = ConsolidateCaret(Text, CaretPos);
        }

    }
}
