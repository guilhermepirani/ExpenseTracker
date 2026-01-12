using FluentValidation;

namespace AppCore.GetEntries;

public class GetEntriesQueryValidator
    : AbstractValidator<GetEntriesQuery>
{
    public GetEntriesQueryValidator()
    {
        RuleFor(entry => entry.Id)
            .Must(id => Guid.TryParse(id, out _) || id is null)
            .WithMessage("If you pass an ID it must be of type UUID.");
    }
}