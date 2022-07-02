using Microsoft.AspNetCore.Components;
using Rtrw.Client.Wasm.Components.Base;
using Rtrw.Client.Wasm.Components.Extensions;
using Rtrw.Client.Wasm.Components.Interfaces;
using Rtrw.Client.Wasm.Utilities;

namespace Rtrw.Client.Wasm.Components
{
    public partial class RtrwForm : RtrwComponentBase, IDisposable, IForm
    {

        protected HashSet<string> _errors = new();
        protected HashSet<IFormComponent> _formControls = new();
        private bool _shouldRender = true; // <-- default is true, we need the form children to render
        private Timer _timer;
        private bool _touched = false;
        private bool _valid = true;

        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter]
        public string[] Errors
        {
            get => _errors.ToArray();
            set
            { /* readonly */
            }
        }
        [Parameter] public EventCallback<string[]> ErrorsChanged { get; set; }
        [Parameter]
        public bool IsTouched
        {
            get => _touched;
            set
            {/* readonly parameter! */
            }
        }
        [Parameter] public EventCallback<bool> IsTouchedChanged { get; set; }
        [Parameter]
        public bool IsValid
        {
            get => _valid &&
                ChildForms.All(
                    x
                        => x.IsValid);
            set { _valid = value; }
        }
        [Parameter] public EventCallback<bool> IsValidChanged { get; set; }
#nullable enable
        [Parameter] public object? Model { get; set; }
        [Parameter] public bool? OverrideFieldValidation { get; set; }
        [Parameter] public bool SuppressImplicitSubmission { get; set; } = true;
        [Parameter] public bool SuppressRenderingOnValidation { get; set; } = false;
        [Parameter] public object Validation { get; set; }
        [Parameter] public int ValidationDelay { get; set; } = 300;
        protected string Classname => new CssBuilder("rtrw-form")
            .AddClass(Class)
            .Build();
#nullable disable

        private HashSet<RtrwForm> ChildForms { get; set; } = new HashSet<RtrwForm>();
        [CascadingParameter] private RtrwForm ParentRtrwForm { get; set; }

        void IForm.Add(IFormComponent formControl)
        {
            if (formControl.Required)
                SetIsValid(false);
            _formControls.Add(formControl);
        }
        void IForm.Remove(IFormComponent formControl) { _formControls.Remove(formControl); }
        void IForm.Update(IFormComponent formControl) { EvaluateForm(); }

        public void Dispose() { _timer?.Dispose(); }
        public void Reset()
        {
            foreach (var control in _formControls.ToArray())
            {
                control.Reset();
            }

            foreach (var form in ChildForms)
            {
                form.Reset();
            }

            EvaluateForm(debounce: false);
        }
        public void ResetValidation()
        {
            foreach (var control in _formControls.ToArray())
            {
                control.ResetValidation();
            }

            foreach (var form in ChildForms)
            {
                form.ResetValidation();
            }

            EvaluateForm(debounce: false);
        }
        public async Task Validate()
        {
            await Task.WhenAll(
                _formControls.Select(
                    x
                        => x.Validate()));

            if (ChildForms.Count > 0)
            {
                await Task.WhenAll(
                    ChildForms.Select(
                        x
                            => x.Validate()));
            }

            EvaluateForm(debounce: false);
        }
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var valid = _formControls.All(
                    x
                        => x.Required == false);
                if (valid != IsValid)
                {
                    // the user probably bound a variable to IsValid and it conflicts with our state.
                    // let's set this right
                    SetIsValid(valid);
                }

                SetDefaultControlValidation(Validation, OverrideFieldValidation ?? true);
            }
            return base.OnAfterRenderAsync(firstRender);
        }
        protected async Task OnEvaluateForm()
        {
            _errors.Clear();
            foreach (var error in _formControls.SelectMany(
                control
                    => control.ValidationErrors))
                _errors.Add(error);
            // form can only be valid if:
            // - none have an error
            // - all required fields have been touched (and thus validated)
            var no_errors = _formControls.All(
                x
                    => x.HasErrors == false);
            var required_all_touched = _formControls.Where(
                x
                    => x.Required)
                .All(
                    x
                        => x.Touched);
            var valid = no_errors && required_all_touched;

            var old_touched = _touched;
            _touched = _formControls.Any(
                x
                    => x.Touched);
            try
            {
                _shouldRender = false;
                SetIsValid(valid);
                await ErrorsChanged.InvokeAsync(Errors);
                if (old_touched != _touched)
                    await IsTouchedChanged.InvokeAsync(_touched);
            }
            finally
            {
                _shouldRender = true;
            }
        }
        protected override void OnInitialized()
        {
            if (ParentRtrwForm != null)
            {
                ParentRtrwForm.ChildForms.Add(this);
            }

            base.OnInitialized();
        }
        protected override bool ShouldRender()
        {
            if (!SuppressRenderingOnValidation)
                return true;
            return _shouldRender;
        }
        private void EvaluateForm(bool debounce = true)
        {
            _timer?.Dispose();
            if (debounce && ValidationDelay > 0)
                _timer = new Timer(OnTimerComplete, null, ValidationDelay, Timeout.Infinite);
            else
                _ = OnEvaluateForm();
        }
        private void OnTimerComplete(object stateInfo) => InvokeAsync(OnEvaluateForm);
        private void SetDefaultControlValidation(object validation, bool overrideFieldValidation)
        {
            if (validation == null)
            {
                return;
            }

            foreach (var formControl in _formControls)
            {
                if (formControl.Validation == null || overrideFieldValidation)
                {
                    formControl.Validation = validation;
                }
            }
        }
        private void SetIsValid(bool value)
        {
            if (IsValid == value)
                return;
            IsValid = value;
            IsValidChanged.InvokeAsync(IsValid).AndForget();
        }

    }
}
