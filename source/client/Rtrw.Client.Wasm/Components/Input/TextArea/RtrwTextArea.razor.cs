using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Rtrw.Client.Wasm.Components.Enums;
using Rtrw.Client.Wasm.Components.Input;
using Rtrw.Client.Wasm.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Components
{
    public partial class RtrwTextArea<T> : RtrwBaseInput<T>
    {

        [Parameter] public bool Clearable { get; set; } = false;
        public RtrwInputTextarea<string>? InputReference { get; private set; }
        [Parameter] public InputType InputType { get; set; } = InputType.Text;
        [Parameter] public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }
        protected string Classname
            => new CssBuilder("rtrw-input-input-control")
                .AddClass(Class)
                .Build();

        public Task Clear()
        {
            return InputReference.SetText(null);
        }
        public override ValueTask FocusAsync()
        {
            return InputReference.FocusAsync();
        }
        public override ValueTask SelectAsync()
        {
            return InputReference.SelectAsync();
        }
        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return InputReference.SelectRangeAsync(pos1, pos2);
        }
        public async Task SetText(string text)
        {
            if (InputReference != null)
                await InputReference.SetText(text);
            return;
        }
        internal override InputType GetInputType() => InputType;
        protected override void ResetValue()
        {
            InputReference.Reset();
            base.ResetValue();
        }
        protected override Task SetTextAsync(string text, bool updateValue = true)
        {
            return base.SetTextAsync(text, updateValue);
        }
        protected override Task SetValueAsync(T value, bool updateText = true)
        {
            return base.SetValueAsync(value, updateText);
        }
        private string GetCounterText()
            => Counter == null ? string.Empty
                : (Counter == 0 ? (string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}")
                    : ((string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") + $" / {Counter}"));
    }
}
