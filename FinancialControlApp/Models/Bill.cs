namespace FinancialControlApp.Models;

internal sealed class Bill
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateOnly DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
