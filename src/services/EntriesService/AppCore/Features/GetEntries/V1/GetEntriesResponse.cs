namespace AppCore.Features.GetEntries.V1;

public class GetEntriesResponse
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public Decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }

    public Entry MapToEntry()
    {
        return new Entry
        {
            Id = this.Id,
            Title = this.Title,
            Amount = this.Amount,
            Description = this.Description,
            Date = this.Date
        };
    }
}