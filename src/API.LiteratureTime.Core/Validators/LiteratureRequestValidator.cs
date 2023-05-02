using API.LiteratureTime.Core.Models;
using FluentValidation;

namespace API.LiteratureTime.Core.Validators;

public class LiteratureRequestValidator : AbstractValidator<LiteratureRequest>
{
    public LiteratureRequestValidator()
    {
        RuleFor(x => x.hash).NotEmpty();
    }
}
