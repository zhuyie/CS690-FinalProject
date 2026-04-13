using FinancialControlApp.Models;
using FinancialControlApp.Persistence;

namespace FinancialControlApp.Services;

internal sealed class BudgetService(JsonDataStore dataStore, TransactionService transactionService)
{
    public Budget SetBudget(string category, decimal monthlyLimit)
    {
        var budgets = dataStore.LoadBudgets();
        var existingBudget = budgets.FirstOrDefault(b => string.Equals(b.Category, category, StringComparison.OrdinalIgnoreCase));

        if (existingBudget is null)
        {
            existingBudget = new Budget
            {
                Category = category,
                MonthlyLimit = monthlyLimit
            };
            budgets.Add(existingBudget);
        }
        else
        {
            existingBudget.MonthlyLimit = monthlyLimit;
        }

        dataStore.SaveBudgets(budgets);
        return existingBudget;
    }

    public IReadOnlyList<BudgetStatus> GetBudgetStatuses()
    {
        return dataStore.LoadBudgets()
            .Select(CreateBudgetStatus)
            .OrderBy(status => status.Category)
            .ToList();
    }

    private BudgetStatus CreateBudgetStatus(Budget budget)
    {
        var spent = transactionService.GetTransactions()
            .Where(transaction => string.Equals(transaction.Category, budget.Category, StringComparison.OrdinalIgnoreCase))
            .Sum(transaction => transaction.Amount);

        var remaining = budget.MonthlyLimit - spent;
        var usageRatio = budget.MonthlyLimit == 0 ? 0 : spent / budget.MonthlyLimit;

        var status = usageRatio switch
        {
            >= 1m => "Over limit",
            >= 0.8m => "Near limit",
            _ => "On track"
        };

        return new BudgetStatus(
            budget.Category,
            budget.MonthlyLimit,
            spent,
            remaining,
            status);
    }
}
