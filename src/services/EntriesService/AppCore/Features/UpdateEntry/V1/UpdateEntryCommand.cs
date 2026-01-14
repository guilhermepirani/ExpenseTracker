using AppCore.Features.GetEntries.V1;
using Mediator.Commands;

namespace AppCore.Features.UpdateEntry.V1;

public class UpdateEntryCommand : ICommand<Result<UpdateEntryResponse>>
{
    public required Guid Id { get; set; }
    public string? Title { get; set; } = null;
    public Decimal? Amount { get; set; } = null;
    public string? Description { get; set; } = null;
    public DateTime? Date { get; set; } = null;

    public Entry UpdateEntry(Entry entry)
    {
        entry.Id = entry.Id;

        entry.Title = this.Title is not null
            ? this.Title
            : entry.Title;

        entry.Amount = this.Amount is not null
            ? (Decimal)this.Amount
            : entry.Amount;

        entry.Description = this.Description is not null
            ? this.Description
            : entry.Description;

        entry.Date = this.Date is not null
            ? (DateTime)this.Date
            : entry.Date;

        return entry;
    }
}