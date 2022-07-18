using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Rtrw.Client.Wasm.Components.Base;
using Rtrw.Client.Wasm.Components.Enums;
using Rtrw.Client.Wasm.Components.Extensions;
using Rtrw.Client.Wasm.Components.Utilities;
using System.Globalization;

namespace Rtrw.Client.Wasm.Components.Input
{
    public abstract class RtrwBaseInput<T> : RtrwFormComponent<T, string>
    {
        protected bool forceTextUpdate;
        protected bool isFocused;

        protected RtrwBaseInput() : base(new DefaultConverter<T>())
        {
        }

        [Parameter] public Adornment Adornment { get; set; } = Adornment.None;
        [Parameter] public bool AutoFocus { get; set; }
        [Parameter] public int? Counter { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public bool DisableUnderLine { get; set; }
        [Parameter] public string Format { get => ((Converter<T>)Converter).Format; set => SetFormat(value); }
        [Parameter] public bool FullWidth { get; set; }
        [Parameter] public string HelperText { get; set; }
        [Parameter] public bool HelperTextOnFocus { get; set; }
        [Parameter] public bool Immediate { get; set; }
        [Parameter] public virtual InputMode InputMode { get; set; } = InputMode.text;
        [Parameter] public bool KeyDownPreventDefault { get; set; }
        [Parameter] public bool KeyPressPreventDefault { get; set; }
        [Parameter] public bool KeyUpPreventDefault { get; set; }
        [Parameter] public string Label { get; set; }
        [Parameter] public int Lines { get; set; } = 1;
        [Parameter] public Margin Margin { get; set; } = Margin.None;
        [Parameter] public int MaxLength { get; set; } = 524288;
        [Parameter] public EventCallback<MouseEventArgs> OnAdornmentClick { get; set; }
        [Parameter] public EventCallback<FocusEventArgs> OnBlur { get; set; }
        [Parameter] public EventCallback<ChangeEventArgs> OnInternalInputChanged { get; set; }
        [Parameter] public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }
        [Parameter] public EventCallback<KeyboardEventArgs> OnKeyPress { get; set; }
        [Parameter] public EventCallback<KeyboardEventArgs> OnKeyUp { get; set; }
        [Parameter] public virtual string Pattern { get; set; }
        [Parameter] public string Placeholder { get; set; }
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public string Text { get; set; }
        [Parameter] public EventCallback<string> TextChanged { get; set; }
        [Parameter] public bool TextUpdateSuppression { get; set; } = true;
        [Parameter] public T Value { get => _value; set => _value = value; }
        [Parameter] public EventCallback<T> ValueChanged { get; set; }
        [Parameter] public Variant Variant { get; set; } = Variant.Text;

        public virtual ValueTask FocusAsync() { return new ValueTask(); }

        public virtual void ForceRender(bool _forceTextUpdate)
        {
            forceTextUpdate = true;
            UpdateTextPropertyAsync(false).AndForget();
            StateHasChanged();
        }

        public virtual ValueTask SelectAsync() { return new ValueTask(); }

        public virtual ValueTask SelectRangeAsync(int pos1, int pos2) { return new ValueTask(); }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            var hasText = parameters.Contains<string>(nameof(Text));
            var hasValue = parameters.Contains<T>(nameof(Value));

            if(hasText && !hasValue)
                await UpdateValuePropertyAsync(false);

            if(hasValue && !hasText)
            {
                var updateText = true;
                if(isFocused && !forceTextUpdate)
                {
                    if(RuntimeLocation.IsServerSide && TextUpdateSuppression)
                        updateText = false;
                }
                if(updateText)
                {
                    forceTextUpdate = false;
                    await UpdateTextPropertyAsync(false);
                }
            }
        }

        internal virtual InputType GetInputType() { return InputType.Text; }

        protected internal virtual void OnBlurred(FocusEventArgs obj)
        {
            isFocused = false;
            Touched = true;
            BeginValidateAfter(OnBlur.InvokeAsync(obj));
        }

        protected virtual void InvokeKeyDown(KeyboardEventArgs obj)
        {
            isFocused = true;
            OnKeyDown.InvokeAsync(obj).AndForget();
        }

        protected virtual void InvokeKeyPress(KeyboardEventArgs obj) { OnKeyPress.InvokeAsync(obj).AndForget(); }

        protected virtual void InvokeKeyUp(KeyboardEventArgs obj)
        {
            isFocused = true;
            OnKeyUp.InvokeAsync(obj).AndForget();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            //Only focus automatically after the first render cycle!
            if(firstRender && AutoFocus)
            {
                await FocusAsync();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if(typeof(T) != typeof(string))
                await UpdateTextPropertyAsync(false);
        }

        protected override void OnParametersSet()
        {
            if(Standalone)
                base.OnParametersSet();
        }

        protected override void ResetValue()
        {
            SetTextAsync(null, updateValue: true).AndForget();
            base.ResetValue();
        }

        protected override bool SetConverter(Utilities.Converter<T, string> value)
        {
            var changed = base.SetConverter(value);
            if(changed)
                UpdateTextPropertyAsync(false).AndForget();      // refresh only Text property from current Value

            return changed;
        }

        protected override bool SetCulture(CultureInfo value)
        {
            var changed = base.SetCulture(value);
            if(changed)
                UpdateTextPropertyAsync(false).AndForget();      // refresh only Text property from current Value

            return changed;
        }

        protected virtual bool SetFormat(string value)
        {
            var changed = Format != value;
            if(changed)
            {
                ((Converter<T>)Converter).Format = value;
                UpdateTextPropertyAsync(false).AndForget();      // refresh only Text property from current Value
            }
            return changed;
        }

        protected virtual async Task SetTextAsync(string text, bool updateValue = true)
        {
            if(Text != text)
            {
                Text = text;
                if(!string.IsNullOrWhiteSpace(Text))
                    Touched = true;
                if(updateValue)
                    await UpdateValuePropertyAsync(false);
                await TextChanged.InvokeAsync(Text);
            }
        }

        protected virtual async Task SetValueAsync(T value, bool updateText = true)
        {
            if(!EqualityComparer<T>.Default.Equals(Value, value))
            {
                Value = value;
                if(updateText)
                    await UpdateTextPropertyAsync(false);
                await ValueChanged.InvokeAsync(Value);
                BeginValidate();
            }
        }

        protected virtual Task UpdateTextPropertyAsync(bool updateValue)
        { return SetTextAsync(Converter.Set(Value), updateValue); }

        protected virtual Task UpdateValuePropertyAsync(bool updateText)
        { return SetValueAsync(Converter.Get(Text), updateText); }

        protected override Task ValidateValue()
        {
            if(Standalone)
                return base.ValidateValue();

            return Task.CompletedTask;
        }
    }
}
