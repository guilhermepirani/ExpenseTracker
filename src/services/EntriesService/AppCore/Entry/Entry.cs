using AppCore.Shared;

namespace AppCore.Entry;

public class Entry : AuditableEntity
{
    public string? Title { get; set; }
    public decimal Amount { get; set; }
    public DateTime EntryDate { get; set; }
    public string? Description { get; set; }
}