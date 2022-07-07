using FluentValidation;
using Rtrw.Client.Wasm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Validators
{
    public class PollFluentValidator : AbstractValidator<Poll>
    {
        public PollFluentValidator()
        {
            RuleFor(x => x.Question).NotEmpty().WithMessage("Pertanyaan tidak boleh kosong");
            RuleForEach(x => x.Answers).NotEmpty().WithMessage("Jawaban tidak boleh kosong");
        }
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue 
            => async (model, propertyName)
                =>
            {
                var result = await ValidateAsync(ValidationContext<Poll>
                        .CreateWithOptions((Poll)model, x => x.IncludeProperties(propertyName)));
                if (result.IsValid)
                    return Array.Empty<string>();
                return result.Errors.Select(e => e.ErrorMessage);
            };
    }
}
