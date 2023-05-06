using API.LiteratureTime.Core.Models;
using FluentValidation;

namespace API.LiteratureTime.Core.Validators;

public class RandomLiteratureRequestValidator : AbstractValidator<RandomLiteratureRequest>
{
    public RandomLiteratureRequestValidator()
    {
        RuleFor(x => x.Hour).Length(2).WithMessage("'{PropertyName}' must be padded with 0");

        RuleFor(x => x.Hour)
            .Must(BeAValidHour)
            .WithMessage("'{PropertyName}' must be between 0 and 23");

        RuleFor(x => x.Minute).Length(2).WithMessage("'{PropertyName}' must be padded with 0");

        RuleFor(x => x.Minute)
            .Must(BeAValidMinute)
            .WithMessage("'{PropertyName}' must be between 0 and 59");
    }

    private bool BeAValidHour(string hour)
    {
        var result = int.TryParse(hour, out int hourAsInteger);
        if (!result)
        {
            return false;
        }

        if (hourAsInteger < 0)
        {
            return false;
        }

        if (hourAsInteger > 23)
        {
            return false;
        }

        return true;
    }

    private bool BeAValidMinute(string minute)
    {
        var result = int.TryParse(minute, out int minuteAsInteger);
        if (!result)
        {
            return false;
        }

        if (minuteAsInteger < 0)
        {
            return false;
        }

        if (minuteAsInteger > 59)
        {
            return false;
        }

        return true;
    }
}
