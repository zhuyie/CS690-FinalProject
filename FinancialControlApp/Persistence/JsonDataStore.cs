using System.Text.Json;
using FinancialControlApp.Models;

namespace FinancialControlApp.Persistence;

internal sealed class JsonDataStore
{
    private readonly string _dataDirectory = Path.Combine(AppContext.BaseDirectory, "data");
    private readonly string _transactionsPath;
    private readonly string _budgetsPath;
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public JsonDataStore()
    {
        _transactionsPath = Path.Combine(_dataDirectory, "transactions.json");
        _budgetsPath = Path.Combine(_dataDirectory, "budgets.json");
    }

    public List<Transaction> LoadTransactions()
    {
        Directory.CreateDirectory(_dataDirectory);

        if (!File.Exists(_transactionsPath))
        {
            return [];
        }

        try
        {
            var json = File.ReadAllText(_transactionsPath);
            return JsonSerializer.Deserialize<List<Transaction>>(json, _jsonOptions) ?? [];
        }
        catch
        {
            return [];
        }
    }

    public void SaveTransactions(List<Transaction> transactions)
    {
        Directory.CreateDirectory(_dataDirectory);
        var json = JsonSerializer.Serialize(transactions, _jsonOptions);
        File.WriteAllText(_transactionsPath, json);
    }

    public List<Budget> LoadBudgets()
    {
        Directory.CreateDirectory(_dataDirectory);

        if (!File.Exists(_budgetsPath))
        {
            return [];
        }

        try
        {
            var json = File.ReadAllText(_budgetsPath);
            return JsonSerializer.Deserialize<List<Budget>>(json, _jsonOptions) ?? [];
        }
        catch
        {
            return [];
        }
    }

    public void SaveBudgets(List<Budget> budgets)
    {
        Directory.CreateDirectory(_dataDirectory);
        var json = JsonSerializer.Serialize(budgets, _jsonOptions);
        File.WriteAllText(_budgetsPath, json);
    }
}
