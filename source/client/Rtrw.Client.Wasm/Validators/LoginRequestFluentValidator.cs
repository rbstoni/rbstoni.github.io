using FluentValidation;
using Rtrw.Client.Wasm.ViewModels;

namespace Rtrw.Client.Wasm.Validators
{
    public class LoginRequestFluentValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestFluentValidator()
        {
            RuleFor(x => x.Phone)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithName("Ponsel").WithMessage("Nomor ponsel tidak boleh kosong");
            RuleFor(x => x.Password)
                .NotEmpty().WithName("Kata Sandi").WithMessage("Kata sandi tidak boleh kosong");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(ValidationContext<LoginRequest>
                                .CreateWithOptions((LoginRequest)model, x => x.IncludeProperties(propertyName)));
                if (result.IsValid)
                    return Array.Empty<string>();
                return result.Errors.Select(e => e.ErrorMessage);
            };
    }
}
