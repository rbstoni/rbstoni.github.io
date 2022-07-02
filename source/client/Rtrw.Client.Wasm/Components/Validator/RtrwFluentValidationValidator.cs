using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Rtrw.Client.Wasm.Components.Extensions;

namespace Rtrw.Client.Wasm.Components
{
    public class RtrwFluentValidationValidator : ComponentBase
    {

        [Parameter] public bool DisableAssemblyScanning { get; set; }
        [Parameter] public Action<ValidationStrategy<object>>? Options { get; set; }
        [Parameter] public IValidator? Validator { get; set; }
        internal Action<ValidationStrategy<object>>? ValidateOptions { get; set; }
        [CascadingParameter] private EditContext? CurrentEditContext { get; set; }
        [Inject] private IServiceProvider ServiceProvider { get; set; } = default!;

        public bool Validate(Action<ValidationStrategy<object>>? options = null)
        {
            if (CurrentEditContext is null)
            {
                throw new NullReferenceException(nameof(CurrentEditContext));
            }

            ValidateOptions = options;

            try
            {
                return CurrentEditContext.Validate();
            }
            finally
            {
                ValidateOptions = null;
            }
        }

        public async Task<bool> ValidateAsync(Action<ValidationStrategy<object>>? options = null)
        {
            if (CurrentEditContext is null)
            {
                throw new NullReferenceException(nameof(CurrentEditContext));
            }

            ValidateOptions = options;

            try
            {
                CurrentEditContext.Validate();

                if (!CurrentEditContext!.Properties.TryGetValue(
                        EditContextFluentValidationExtensions.PendingAsyncValidation, out var asyncValidationTask))
                {
                    throw new InvalidOperationException("No pending ValidationResult found");
                }

                await (Task<ValidationResult>)asyncValidationTask;

                return !CurrentEditContext.GetValidationMessages().Any();
            }
            finally
            {
                ValidateOptions = null;
            }
        }
        protected override void OnInitialized()
        {
            if (CurrentEditContext == null)
            {
                throw new InvalidOperationException($"{nameof(RtrwFluentValidationValidator)} requires a cascading " +
                                                    $"parameter of type {nameof(EditContext)}. For example, you can use {nameof(RtrwFluentValidationValidator)} " +
                                                    $"inside an {nameof(EditForm)}.");
            }

            CurrentEditContext.AddFluentValidation(ServiceProvider, DisableAssemblyScanning, Validator, this);
        }

    }
}
