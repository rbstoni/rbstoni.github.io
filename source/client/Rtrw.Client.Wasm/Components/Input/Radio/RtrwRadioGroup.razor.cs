using Microsoft.AspNetCore.Components;
using Rtrw.Client.Wasm.Components;
using Rtrw.Client.Wasm.Components.Base;
using Rtrw.Client.Wasm.Components.Extensions;
using Rtrw.Client.Wasm.Components.Input.Radio;
using Rtrw.Client.Wasm.Components.Utilities;
using Rtrw.Client.Wasm.Components.Utilities.Exceptions;

namespace Rtrw.Client.Wasm.Components
{
    public partial class RtrwRadioGroup<T> : RtrwFormComponent<T, T>, IRtrwRadioGroup
    {

        private HashSet<RtrwRadio<T>> _radios = new();
        private RtrwRadio<T> _selectedRadio;

        public RtrwRadioGroup() : base(new Wasm.Components.Utilities.Converter<T, T>()) { }

        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public string Name { get; set; } = Guid.NewGuid().ToString();
        [Parameter]
        public T SelectedOption
        {
            get => _value;
            set => SetSelectedOptionAsync(value, true).AndForget();
        }
        [Parameter] public EventCallback<T> SelectedOptionChanged { get; set; }

        public void CheckGenericTypeMatch(object select_item)
        {
            var itemT = select_item.GetType().GenericTypeArguments[0];
            if (itemT != typeof(T))
                throw new GenericTypeMismatchException("RtrwRadioGroup", "RtrwRadio", typeof(T), itemT);
        }
        internal Task RegisterRadioAsync(RtrwRadio<T> radio)
        {
            _radios.Add(radio);

            if (_selectedRadio == null)
            {
                if (OptionEquals(radio.Option, _value))
                    return SetSelectedRadioAsync(radio, false);
            }
            return Task.CompletedTask;
        }
        internal Task SetSelectedRadioAsync(RtrwRadio<T> radio)
        {
            Touched = true;
            return SetSelectedRadioAsync(radio, true);
        }
        internal void UnregisterRadio(RtrwRadio<T> radio)
        {
            _radios.Remove(radio);

            if (_selectedRadio == radio)
                _selectedRadio = null;
        }
        protected override void ResetValue()
        {
            if (_selectedRadio != null)
            {
                _selectedRadio.SetChecked(false);
                _selectedRadio = null;
            }

            base.ResetValue();
        }
        protected async Task SetSelectedOptionAsync(T option, bool updateRadio)
        {
            if (!OptionEquals(_value, option))
            {
                _value = option;

                if (updateRadio)
                    await SetSelectedRadioAsync(_radios.FirstOrDefault(r => OptionEquals(r.Option, _value)), false);

                await SelectedOptionChanged.InvokeAsync(_value);

                BeginValidate();
            }
        }
        protected async Task SetSelectedRadioAsync(RtrwRadio<T> radio, bool updateOption)
        {
            if (_selectedRadio != radio)
            {
                _selectedRadio = radio;

                foreach (var item in _radios.ToArray())
                    item.SetChecked(item == _selectedRadio);

                if (updateOption)
                    await SetSelectedOptionAsync(GetOptionOrDefault(_selectedRadio), false);
            }
        }
        private static T GetOptionOrDefault(RtrwRadio<T> radio)
        {
            return radio != null ? radio.Option : default;
        }
        private static bool OptionEquals(T option1, T option2)
        {
            return EqualityComparer<T>.Default.Equals(option1, option2);
        }

    }
}
