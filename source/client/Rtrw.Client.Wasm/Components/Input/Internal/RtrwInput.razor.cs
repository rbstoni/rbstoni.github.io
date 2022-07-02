using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Rtrw.Client.Wasm.Components.Enums;
using Rtrw.Client.Wasm.Components.Extensions;
using Rtrw.Client.Wasm.Components.Input.Base;
using Rtrw.Client.Wasm.Components.Utilities;
using Rtrw.Client.Wasm.Utilities;

namespace Rtrw.Client.Wasm.Components.Input.Internal
{
    public partial class RtrwInput<T> : RtrwBaseInput<T>
    {
        private ElementReference elementReference1;
        private string internalText;
        private bool showClearable;

        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public bool Clearable { get; set; } = false;
        public ElementReference ElementReference { get; private set; }
        [Parameter] public bool HideSpinButtons { get; set; } = true;
        [Parameter] public InputType InputType { get; set; } = InputType.Text;
        [Parameter] public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }
        [Parameter] public RenderFragment AdornmentContent { get; set; }
        [Parameter] public RenderFragment AdornmentSvg { get; set; }
        [Parameter] public EventCallback OnDecrement { get; set; }
        //public override ValueTask SelectAsync()
        //{
        //    return ElementReference.RtrwSelectAsync();
        //}
        //public override ValueTask SelectRangeAsync(int pos1, int pos2)
        //{
        //    return ElementReference.RtrwSelectRangeAsync(pos1, pos2);
        //}
        [Parameter] public EventCallback OnIncrement { get; set; }
        [Parameter] public EventCallback<WheelEventArgs> OnMouseWheel { get; set; }
        protected string AdornmentClassname => RtrwInputCssHelper.GetAdornmentClassname(this);
        protected string Classname => RtrwInputCssHelper.GetClassname(
            this,
            ()
                => HasNativeHtmlPlaceholder() ||
                !string.IsNullOrEmpty(Text) ||
                Adornment == Adornment.Start ||
                !string.IsNullOrWhiteSpace(Placeholder));
        protected string ClearButtonClassname => new CssBuilder()
                    .AddClass("rtrw-margin-end-4px", Adornment == Adornment.End && HideSpinButtons == false)
            .AddClass("rtrw-icon-button-edge-end", Adornment == Adornment.End && HideSpinButtons == true)
            .AddClass("rtrw-margin-end-24px", Adornment != Adornment.End && HideSpinButtons == false)
            .AddClass("rtrw-icon-button-edge-margin-end", Adornment != Adornment.End && HideSpinButtons == true)
            .Build();
        protected string InputClassname => RtrwInputCssHelper.GetInputClassname(this);
        protected string InputTypeString => InputType.EnumToDescriptionString();

        public override async ValueTask FocusAsync()
        {
            try
            {
                if(InputType == InputType.Hidden && ChildContent != null)
                    await elementReference1.FocusAsync();
                else
                    await ElementReference.FocusAsync();
            } catch(Exception e)
            {
                Console.WriteLine("RtrwInput.FocusAsync: " + e.Message);
            }
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            //if (!isFocused || forceTextUpdate)
            //    internalText = Text;
            if(RuntimeLocation.IsServerSide && TextUpdateSuppression)
            {
                // Text update suppression, only in BSS (not in WASM).
                // This is a fix for #1012
                if(!isFocused || forceTextUpdate)
                    internalText = Text;
            } else
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
            if(!Immediate)
            {
                await SetTextAsync(args?.Value as string);
            }
        }

        protected Task OnInput(ChangeEventArgs args)
        {
            if(!Immediate)
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
            if(Clearable)
                UpdateClearable(Text);
        }

        protected override async Task UpdateValuePropertyAsync(bool updateText)
        {
            await base.UpdateValuePropertyAsync(updateText);
            if(Clearable)
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
            if(showClearable != _showClearable)
                showClearable = _showClearable;
        }
    }

    public class RtrwInputString : RtrwInput<string>
    {
    }
}
