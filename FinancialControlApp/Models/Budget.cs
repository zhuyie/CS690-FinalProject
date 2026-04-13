namespace FinancialControlApp.Models;

internal sealed class Budget
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Category { get; set; } = string.Empty;
    public decimal MonthlyLimit { get; set; }
}
