using System;

namespace AppCore.GetEntries;

public class GetEntriesResponse
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public Decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset Date { get; set; }
}