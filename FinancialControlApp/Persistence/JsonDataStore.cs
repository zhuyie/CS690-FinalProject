using System.Text.Json;
using FinancialControlApp.Models;

namespace FinancialControlApp.Persistence;

internal sealed class JsonDataStore
{
    private readonly string _dataDirectory;
    private readonly string _transactionsPath;
    private readonly string _budgetsPath;
    private readonly string _billsPath;
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public JsonDataStore(string? baseDirectory = null)
    {
        _dataDirectory = Path.Combine(baseDirectory ?? AppContext.BaseDirectory, "data");
        _transactionsPath = Path.Combine(_dataDirectory, "transactions.json");
        _budgetsPath = Path.Combine(_dataDirectory, "budgets.json");
        _billsPath = Path.Combine(_dataDirectory, "bills.json");
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

    public List<Bill> LoadBills()
    {
        Directory.CreateDirectory(_dataDirectory);

        if (!File.Exists(_billsPath))
        {
            return [];
        }

        try
        {
            var json = File.ReadAllText(_billsPath);
            return JsonSerializer.Deserialize<List<Bill>>(json, _jsonOptions) ?? [];
        }
        catch
        {
            return [];
        }
    }

    public void SaveBills(List<Bill> bills)
    {
        Directory.CreateDirectory(_dataDirectory);
        var json = JsonSerializer.Serialize(bills, _jsonOptions);
        File.WriteAllText(_billsPath, json);
    }
}
