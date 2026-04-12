using System.Text.Json;

var app = new FinancialControlApp();
app.Run();

internal sealed class FinancialControlApp
{
    private static readonly string[] Categories =
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

    private readonly string _dataDirectory;
    private readonly string _transactionsPath;
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    private List<Transaction> _transactions = [];

    public FinancialControlApp()
    {
        _dataDirectory = Path.Combine(AppContext.BaseDirectory, "data");
        _transactionsPath = Path.Combine(_dataDirectory, "transactions.json");
    }

    public void Run()
    {
        LoadTransactions();

        while (true)
        {
            ShowHeader("Financial Control for Daniel");
            Console.WriteLine("1. Log Daily Expense");
            Console.WriteLine("2. View Transaction History");
            Console.WriteLine("3. View Spending Breakdown");
            Console.WriteLine("4. Exit");
            Console.WriteLine();

            var choice = Prompt("Select an option: ");
            Console.Clear();

            switch (choice)
            {
                case "1":
                    LogExpense();
                    break;
                case "2":
                    ShowTransactionHistory();
                    break;
                case "3":
                    ShowSpendingBreakdown();
                    break;
                case "4":
                    Console.WriteLine("Goodbye.");
                    return;
                default:
                    ShowMessage("Invalid option. Please choose 1-4.");
                    break;
            }
        }
    }

    private void LogExpense()
    {
        ShowHeader("Log Daily Expense");

        var amount = PromptForAmount();
        var category = PromptForCategory();
        var date = PromptForDate();
        var description = PromptForRequiredText("Enter a short description: ");

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Amount = amount,
            Category = category,
            Date = date,
            Description = description
        };

        _transactions.Add(transaction);
        SaveTransactions();

        Console.WriteLine();
        Console.WriteLine("Expense saved successfully.");
        Console.WriteLine($"{transaction.Date:yyyy-MM-dd} | {transaction.Category} | ${transaction.Amount:F2} | {transaction.Description}");
        Pause();
    }

    private void ShowTransactionHistory()
    {
        ShowHeader("Transaction History");

        if (_transactions.Count == 0)
        {
            ShowMessage("No transactions found.");
            return;
        }

        var orderedTransactions = _transactions
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.CreatedAt)
            .ToList();

        Console.WriteLine("Date         Category        Amount     Description");
        Console.WriteLine("-------------------------------------------------------------");

        foreach (var transaction in orderedTransactions)
        {
            Console.WriteLine(
                $"{transaction.Date:yyyy-MM-dd}   " +
                $"{transaction.Category.PadRight(13)} " +
                $"${transaction.Amount,8:F2}   " +
                $"{transaction.Description}");
        }

        Pause();
    }

    private void ShowSpendingBreakdown()
    {
        ShowHeader("Spending Breakdown");

        if (_transactions.Count == 0)
        {
            ShowMessage("No transactions found.");
            return;
        }

        var groups = _transactions
            .GroupBy(t => t.Category)
            .Select(group => new
            {
                Category = group.Key,
                Total = group.Sum(t => t.Amount),
                Count = group.Count()
            })
            .OrderByDescending(group => group.Total)
            .ToList();

        var grandTotal = groups.Sum(group => group.Total);

        Console.WriteLine("Category        Entries     Total");
        Console.WriteLine("--------------------------------------");

        foreach (var group in groups)
        {
            Console.WriteLine(
                $"{group.Category.PadRight(14)}" +
                $"{group.Count,7}   " +
                $"${group.Total,8:F2}");
        }

        Console.WriteLine("--------------------------------------");
        Console.WriteLine($"Monthly Total: ${grandTotal:F2}");
        Pause();
    }

    private decimal PromptForAmount()
    {
        while (true)
        {
            var input = Prompt("Enter amount: $");
            if (decimal.TryParse(input, out var amount) && amount > 0)
            {
                return decimal.Round(amount, 2);
            }

            Console.WriteLine("Please enter a valid positive amount.");
        }
    }

    private string PromptForCategory()
    {
        while (true)
        {
            Console.WriteLine("Select a category:");

            for (var i = 0; i < Categories.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {Categories[i]}");
            }

            var input = Prompt("Choice: ");
            if (int.TryParse(input, out var choice) && choice >= 1 && choice <= Categories.Length)
            {
                return Categories[choice - 1];
            }

            Console.WriteLine("Please select one of the listed categories.");
        }
    }

    private DateOnly PromptForDate()
    {
        while (true)
        {
            var input = Prompt("Enter date (YYYY-MM-DD) or press Enter for today: ");
            if (string.IsNullOrWhiteSpace(input))
            {
                return DateOnly.FromDateTime(DateTime.Today);
            }

            if (DateOnly.TryParse(input, out var date))
            {
                return date;
            }

            Console.WriteLine("Please enter a valid date in YYYY-MM-DD format.");
        }
    }

    private string PromptForRequiredText(string message)
    {
        while (true)
        {
            var input = Prompt(message);
            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.Trim();
            }

            Console.WriteLine("This field cannot be empty.");
        }
    }

    private void LoadTransactions()
    {
        Directory.CreateDirectory(_dataDirectory);

        if (!File.Exists(_transactionsPath))
        {
            _transactions = [];
            return;
        }

        try
        {
            var json = File.ReadAllText(_transactionsPath);
            _transactions = JsonSerializer.Deserialize<List<Transaction>>(json, _jsonOptions) ?? [];
        }
        catch
        {
            _transactions = [];
        }
    }

    private void SaveTransactions()
    {
        Directory.CreateDirectory(_dataDirectory);
        var json = JsonSerializer.Serialize(_transactions, _jsonOptions);
        File.WriteAllText(_transactionsPath, json);
    }

    private static void ShowHeader(string title)
    {
        Console.WriteLine(title);
        Console.WriteLine(new string('=', title.Length));
        Console.WriteLine();
    }

    private static string Prompt(string message)
    {
        Console.Write(message);
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }

    private static void Pause()
    {
        Console.WriteLine();
        Console.Write("Press Enter to continue...");
        Console.ReadLine();
        Console.Clear();
    }

    private static void ShowMessage(string message)
    {
        Console.WriteLine(message);
        Pause();
    }
}

internal sealed class Transaction
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
