namespace AppCore;

public class Entry
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; } = null;
    public DateTime Date { get; set; }
}