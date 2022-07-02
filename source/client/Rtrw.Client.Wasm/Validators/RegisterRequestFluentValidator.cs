using FluentValidation;
using Rtrw.Client.Wasm.ViewModels;

namespace Rtrw.Client.Wasm.Validators
{
    public class RegisterRequestFluentValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestFluentValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email tidak boleh kosong");
            RuleFor(x => x.Phone).NotEmpty().WithMessage("No telp. tidak boleh kosong");
        }
    }
}
