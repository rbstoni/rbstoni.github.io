using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Rtrw.Client.Wasm.Components.Enums;
using Rtrw.Client.Wasm.Components.Input.Base;
using Rtrw.Client.Wasm.Components.Input.Internal;
using Rtrw.Client.Wasm.Components.Mask.MaskAlgorithms;
using Rtrw.Client.Wasm.Utilities;

namespace Rtrw.Client.Wasm.Components
{
    public partial class RtrwTextField<T> : RtrwBaseInput<T>
    {
        private IMask? mask = null;
        private RtrwMask? maskReference;

        [Parameter] public bool Clearable { get; set; } = false;
        public RtrwInput<string>? InputReference { get; private set; }
        [Parameter] public InputType InputType { get; set; } = InputType.Text;
        [Parameter] public RenderFragment? AdornmentContent { get; set; }
        [Parameter] public RenderFragment? AdornmentSvg { get; set; }
        [Parameter]
        public IMask Mask
        {
            get => maskReference?.Mask ?? mask; // this might look strange, but it is absolutely necessary due to how RtrwMask works.
            set { mask = value; }
        }
        [Parameter] public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }
        protected string Classname 
            => new CssBuilder("rtrw-input-input-control")
                .AddClass(Class)
                .Build();

        public Task Clear()
        {
            if(mask == null)
                return InputReference.SetText(null);
            else
                return maskReference.Clear();
        }

        public override ValueTask FocusAsync()
        {
            if(mask == null)
                return InputReference.FocusAsync();
            else
                return maskReference.FocusAsync();
        }

        public override ValueTask SelectAsync()
        {
            if(mask == null)
                return InputReference.SelectAsync();
            else
                return maskReference.SelectAsync();
        }

        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            if(mask == null)
                return InputReference.SelectRangeAsync(pos1, pos2);
            else
                return maskReference.SelectRangeAsync(pos1, pos2);
        }

        public async Task SetText(string text)
        {
            if(mask == null)
            {
                if(InputReference != null)
                    await InputReference.SetText(text);
                return;
            }
            await maskReference.Clear();
            maskReference.OnPaste(text);
        }

        internal override InputType GetInputType() => InputType;

        protected override void ResetValue()
        {
            if(mask == null)
                InputReference.Reset();
            else
                maskReference.Reset();
            base.ResetValue();
        }

        protected override Task SetTextAsync(string text, bool updateValue = true)
        {
            if(mask != null)
            {
                mask.SetText(text);
                text = mask.Text;
            }
            return base.SetTextAsync(text, updateValue);
        }

        protected override Task SetValueAsync(T value, bool updateText = true)
        {
            if(mask != null)
            {
                var textValue = Converter.Set(value);
                mask.SetText(textValue);
                textValue = Mask.GetCleanText();
                value = Converter.Get(textValue);
            }
            return base.SetValueAsync(value, updateText);
        }

        private string GetCounterText() => Counter == null
            ? string.Empty
            : (Counter == 0
                ? (string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}")
                : ((string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") + $" / {Counter}"));

        private async Task OnMaskedValueChanged(string str) { await SetTextAsync(str); }
    }
}
