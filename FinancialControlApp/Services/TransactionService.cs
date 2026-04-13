using FinancialControlApp.Models;
using FinancialControlApp.Persistence;

namespace FinancialControlApp.Services;

internal sealed class TransactionService(JsonDataStore dataStore)
{
    public static readonly string[] DefaultCategories =
    {
        "Coffee",
        "Breakfast",
        "Lunch",
        "Dinner",
        "Groceries",
        "Transport",
        "Utilities",
        "Entertainment",
        "Other"
    };

    public Transaction AddTransaction(decimal amount, string category, DateOnly date, string description)
    {
        var transactions = dataStore.LoadTransactions();
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Amount = amount,
            Category = category,
            Date = date,
            Description = description
        };

        transactions.Add(transaction);
        dataStore.SaveTransactions(transactions);
        return transaction;
    }

    public IReadOnlyList<Transaction> GetTransactions()
    {
        return dataStore.LoadTransactions()
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.CreatedAt)
            .ToList();
    }

    public IReadOnlyList<SpendingSummary> GetSpendingSummaries()
    {
        return dataStore.LoadTransactions()
            .GroupBy(t => t.Category)
            .Select(group => new SpendingSummary(group.Key, group.Count(), group.Sum(t => t.Amount)))
            .OrderByDescending(group => group.Total)
            .ToList();
    }
}
