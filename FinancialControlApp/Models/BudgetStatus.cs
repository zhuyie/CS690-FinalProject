namespace FinancialControlApp.Models;

internal sealed record BudgetStatus(
    string Category,
    decimal MonthlyLimit,
    decimal Spent,
    decimal Remaining,
    string StatusLabel);
