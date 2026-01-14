using FluentValidation;

namespace AppCore.Features.UpdateEntry.V1;

public class UpdateEntryValidator
    : AbstractValidator<UpdateEntryCommand>
{
    public UpdateEntryValidator()
    {
        RuleFor(entry => entry.Title)
            .MinimumLength(1)
            .MaximumLength(50);

        RuleFor(entry => entry.Amount)
            .GreaterThan(0);

        RuleFor(entry => entry.Description)
            .MaximumLength(500);
    }
}