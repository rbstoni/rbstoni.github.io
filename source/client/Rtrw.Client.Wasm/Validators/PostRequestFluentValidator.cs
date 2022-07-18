using FluentValidation;
using Rtrw.Client.Wasm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Validators
{
    public class PostRequestFluentValidator : AbstractValidator<Post>
    {
        public PostRequestFluentValidator()
        {
            RuleFor(x => x.Text).NotEmpty().WithMessage("Konten post tidak boleh kosong");
        }
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<Post>.CreateWithOptions((Post)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
