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

    private static bool BeAValidHour(string hour)
    {
        if (!int.TryParse(hour, out var hourAsInteger))
        {
            return false;
        }

        return hourAsInteger switch
        {
            < 0 or > 23 => false,
            _ => true,
        };
    }

    private static bool BeAValidMinute(string minute)
    {
        if (!int.TryParse(minute, out var minuteAsInteger))
        {
            return false;
        }

        return minuteAsInteger switch
        {
            < 0 or > 59 => false,
            _ => true,
        };
    }
}
