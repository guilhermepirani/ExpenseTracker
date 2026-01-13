using FluentValidation;

namespace AppCore.Features.CreateEntry.V1;

public class CreatyEntryValidator
    : AbstractValidator<CreateEntryCommand>
{
    public CreatyEntryValidator()
    {
        RuleFor(entry => entry.Title)
            .NotNull()
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(entry => entry.Amount)
            .NotNull()
            .GreaterThan(0);

        RuleFor(entry => entry.Description)
            .MaximumLength(500);
    }
}