using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Rtrw.Client.Wasm.Components.Enums;
using Rtrw.Client.Wasm.Components.Extensions;
using Rtrw.Client.Wasm.Components.Utilities;
using Rtrw.Client.Wasm.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Components.Input
{
    public partial class RtrwInputTextarea<T> : RtrwBaseInput<T>
    {

        private ElementReference elementReference1;
        private string? internalText;
        private bool showClearable;

        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public bool Clearable { get; set; } = false;
        public ElementReference ElementReference { get; private set; }
        [Parameter] public InputType InputType { get; set; } = InputType.Text;
        [Parameter] public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }
        [Parameter] public EventCallback OnDecrement { get; set; }
        public override ValueTask SelectAsync()
        {
            return ElementReference.RtrwSelectAsync();
        }
        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return ElementReference.RtrwSelectRangeAsync(pos1, pos2);
        }
        [Parameter] public EventCallback OnIncrement { get; set; }
        [Parameter] public EventCallback<WheelEventArgs> OnMouseWheel { get; set; }
        protected string Classname
            => new CssBuilder("rtrw-input")
                .AddClass($"rtrw-input-{Variant.EnumToDescriptionString()}")
                .AddClass($"rtrw-input-margin-{Margin.EnumToDescriptionString()}", when: () => Margin != Margin.None)
                .AddClass("rtrw-input-underline", when: () => DisableUnderLine == false && Variant != Variant.Outlined)
                .AddClass("rtrw-shrink", when: ShrinkWhen)
                .AddClass("rtrw-disabled", Disabled)
                .AddClass("rtrw-input-error", HasErrors)
                .AddClass(Class)
                .Build();

        Func<bool> ShrinkWhen => () => HasNativeHtmlPlaceholder() || !string.IsNullOrEmpty(Text) || Adornment == Adornment.Start || !string.IsNullOrWhiteSpace(Placeholder);
        protected string ClearButtonClassname
            => new CssBuilder()
                .Build();
        protected string InputClassname
            => new CssBuilder("rtrw-input-slot")
                .AddClass("rtrw-input-root")
                .AddClass($"rtrw-input-root-{Variant.EnumToDescriptionString()}")
                .AddClass($"rtrw-input-root-margin-{Margin.EnumToDescriptionString()}", when: () => Margin != Margin.None)
                .AddClass(Class)
                .Build();
        protected string InputTypeString => InputType.EnumToDescriptionString();

        public override async ValueTask FocusAsync()
        {
            try
            {
                if (InputType == InputType.Hidden && ChildContent != null)
                    await elementReference1.FocusAsync();
                else
                    await ElementReference.FocusAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("RtrwInput.FocusAsync: " + e.Message);
            }
        }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            //if (!isFocused || forceTextUpdate)
            //    internalText = Text;
            if (RuntimeLocation.IsServerSide && TextUpdateSuppression)
            {
                // Text update suppression, only in BSS (not in WASM).
                // This is a fix for #1012
                if (!isFocused || forceTextUpdate)
                    internalText = Text;
            }
            else
            {
                // in WASM (or in BSS with TextUpdateSuppression==false) we always update
                internalText = Text;
            }
        }
        public Task SetText(string text)
        {
            internalText = text;
            return SetTextAsync(text);
        }
        internal override InputType GetInputType() => InputType;
        protected virtual async Task ClearButtonClickHandlerAsync(MouseEventArgs e)
        {
            await SetTextAsync(string.Empty, updateValue: true);
            await ElementReference.FocusAsync();
            await OnClearButtonClick.InvokeAsync(e);
        }
        protected async Task OnChange(ChangeEventArgs args)
        {
            internalText = args?.Value as string;
            await OnInternalInputChanged.InvokeAsync(args);
            if (!Immediate)
            {
                await SetTextAsync(args?.Value as string);
            }
        }
        protected Task OnInput(ChangeEventArgs args)
        {
            if (!Immediate)
                return Task.CompletedTask;
            isFocused = true;
            return SetTextAsync(args?.Value as string);
        }
        protected virtual async Task OnPaste(ClipboardEventArgs args)
        {
            // do nothing
            return;
        }
        protected override async Task UpdateTextPropertyAsync(bool updateValue)
        {
            await base.UpdateTextPropertyAsync(updateValue);
            if (Clearable)
                UpdateClearable(Text);
        }
        protected override async Task UpdateValuePropertyAsync(bool updateText)
        {
            await base.UpdateValuePropertyAsync(updateText);
            if (Clearable)
                UpdateClearable(Value);
        }
        private bool HasNativeHtmlPlaceholder()
        {
            return GetInputType() is InputType.Color or InputType.Date or InputType.DateTimeLocal or InputType.Month
                or InputType.Time or InputType.Week;
        }
        private void UpdateClearable(object value)
        {
            var _showClearable = Clearable &&
                ((value is string stringValue && !string.IsNullOrWhiteSpace(stringValue)) ||
                    (value is not string && value is not null));
            if (showClearable != _showClearable)
                showClearable = _showClearable;
        }

    }
}
