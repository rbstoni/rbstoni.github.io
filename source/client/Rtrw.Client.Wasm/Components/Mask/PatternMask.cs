using Rtrw.Client.Wasm.Components.Interfaces;
using System.Text.RegularExpressions;

namespace Rtrw.Client.Wasm.Components.Mask
{
    public class PatternMask : BaseMask
    {

        public PatternMask(string mask)
        {
            Mask = mask;
        }

        public bool CleanDelimiters { get; set; }
        public char? Placeholder { get; set; }
        public Func<char, char> Transformation { get; set; }

        public override void Backspace()
        {
            Init();
            if (Selection != null)
            {
                DeleteSelection(align: true);
                return;
            }
            var text = Text ?? "";
            var pos = CaretPos = ConsolidateCaret(text, CaretPos);
            if (pos == 0)
                return;
            (var beforeText, var afterText) = SplitAt(text, pos);
            // backspace as many delimiters as there are plus one char
            var restText = new string(beforeText.Reverse().SkipWhile(IsDelimiter).Skip(1).Reverse().ToArray());
            var numDeleted = beforeText.Length - restText.Length;
            CaretPos -= numDeleted;
            var alignedAfter = AlignAgainstMask(afterText, CaretPos);
            UpdateText(FillWithPlaceholder(restText + alignedAfter));
        }
        public override void Delete()
        {
            Init();
            if (Selection != null)
            {
                DeleteSelection(align: true);
                return;
            }
            var text = Text ?? "";
            var pos = CaretPos = ConsolidateCaret(text, CaretPos);
            if (pos >= text.Length)
                return;
            (var beforeText, var afterText) = SplitAt(text, pos);
            // delete as many delimiters as there are plus one char
            var restText = new string(afterText.SkipWhile(IsDelimiter).Skip(1).ToArray());
            var alignedAfter = AlignAgainstMask(restText, pos);
            var numDeleted = afterText.Length - restText.Length;
            if (numDeleted > 1)
            {
                // since we just auto-deleted delimiters which were re-created by AlignAgainstMask we can just as well
                // adjust the cursor position to after the delimiters
                CaretPos += (numDeleted - 1);
            }
            UpdateText(FillWithPlaceholder(beforeText + alignedAfter));
        }
        public override string GetCleanText()
        {
            Init();
            var cleanText = Text;
            if (string.IsNullOrEmpty(cleanText))
                return cleanText;
            if (CleanDelimiters)
                cleanText = new string(cleanText.Where((c, i) => _maskDict.ContainsKey(Mask[i])).ToArray());
            if (Placeholder != null)
                cleanText = cleanText.Replace(Placeholder.Value.ToString(), "");
            return cleanText;
        }
        public override void Insert(string input)
        {
            Init();
            DeleteSelection(align: false);
            var text = Text ?? "";
            var pos = ConsolidateCaret(text, CaretPos);
            (var beforeText, var afterText) = SplitAt(text, pos);
            var alignedBefore = AlignAgainstMask(beforeText, 0);
            CaretPos = pos = alignedBefore.Length;
            var alignedInput = AlignAgainstMask(input, pos);
            CaretPos = pos += alignedInput.Length;
            if (Placeholder != null)
            {
                var p = Placeholder.Value;
                if (afterText.Take(alignedInput.Length).All(c => IsDelimiter(c) || c == p))
                    afterText = new string(afterText.Skip(alignedInput.Length).ToArray());
            }
            var alignedAfter = AlignAgainstMask(afterText, pos);
            UpdateText(FillWithPlaceholder(alignedBefore + alignedInput + alignedAfter));
        }
        public override void UpdateFrom(IMask other)
        {
            base.UpdateFrom(other);
            var o = other as PatternMask;
            if (o == null)
                return;
            Placeholder = o.Placeholder;
            CleanDelimiters = o.CleanDelimiters;
            Transformation = o.Transformation;
            _initialized = false;
            Refresh();
        }
        protected virtual string AlignAgainstMask(string text, int maskOffset = 0)
        {
            text ??= "";
            var mask = Mask ?? "";
            var alignedText = "";
            var maskIndex = maskOffset; // index in mask
            var textIndex = 0; // index in text
            while (textIndex < text.Length)
            {
                if (maskIndex >= mask.Length)
                    break;
                var maskChar = mask[maskIndex];
                var textChar = text[textIndex];
                if (IsDelimiter(maskChar))
                {
                    alignedText += maskChar;
                    maskIndex++;
                    ModifyPartiallyAlignedMask(mask, text, maskOffset, ref textIndex, ref maskIndex, ref alignedText);
                    continue;
                }
                var isPlaceholder = Placeholder != null && textChar == Placeholder.Value;
                if (IsMatch(maskChar, textChar) || isPlaceholder)
                {
                    var c = Transformation == null ? textChar : Transformation(textChar);
                    alignedText += c;
                    maskIndex++;
                }
                textIndex++;
                ModifyPartiallyAlignedMask(mask, text, maskOffset, ref textIndex, ref maskIndex, ref alignedText);
            }
            // fill any delimiters if possible
            for (int i = maskIndex; i < mask.Length; i++)
            {
                var maskChar = mask[i];
                if (!IsDelimiter(maskChar))
                    break;
                alignedText += maskChar;
            }
            return alignedText;
        }
        protected override void DeleteSelection(bool align)
        {
            ConsolidateSelection();
            if (Selection == null)
                return;
            var sel = Selection.Value;
            (var s1, _, var s3) = SplitSelection(Text, sel);
            Selection = null;
            CaretPos = sel.Item1;
            if (!align)
                UpdateText(s1 + s3);
            else
                UpdateText(FillWithPlaceholder(s1 + AlignAgainstMask(s3, CaretPos)));
        }
        protected virtual string FillWithPlaceholder(string text)
        {
            if (Placeholder == null)
                return text;
            // fill the rest with placeholder
            // don't fill if text is still empty
            var filledText = text;
            var len = text.Length;
            var mask = Mask ?? "";
            if (len == 0 || len >= mask.Length)
                return text;
            for (var maskIndex = len; maskIndex < mask.Length; maskIndex++)
            {
                var maskChar = mask[maskIndex];
                if (IsDelimiter(maskChar))
                    filledText += maskChar;
                else
                    filledText += Placeholder.Value;
            }
            return filledText;
        }
        protected override void InitInternals()
        {
            base.InitInternals();
            if (Placeholder != null)
                _delimiters.Add(Placeholder.Value);
        }
        protected virtual bool IsMatch(char maskChar, char textChar)
        {
            var maskDef = _maskDict[maskChar];
            return Regex.IsMatch(textChar.ToString(), maskDef.Regex);
        }
        protected virtual string ModifyFinalText(string text)
        {
            /* this  can be overridden in derived classes to apply any necessary changes to the resulting text */
            return text;
        }
        protected virtual void ModifyPartiallyAlignedMask(string mask, string text, int maskOffset, ref int textIndex, ref int maskIndex, ref string alignedText)
        {
            /* this is an override hook for more specialized mask implementations deriving from this*/
        }
        protected override void UpdateText(string text)
        {
            // don't show a text consisting only of delimiters and placeholders (no actual input)
            if (text.All(c => _delimiters.Contains(c) || (Placeholder != null && c == Placeholder.Value)))
            {
                Text = "";
                CaretPos = 0;
                return;
            }
            Text = ModifyFinalText(text);
            CaretPos = ConsolidateCaret(Text, CaretPos);
        }

    }
}
