using System.Globalization;
using System.Text.Json;
using Spectre.Console;

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
            AnsiConsole.Clear();
            ShowHeader();

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Choose an option[/]")
                    .PageSize(10)
                    .AddChoices(
                        "Log Daily Expense",
                        "View Transaction History",
                        "View Spending Breakdown",
                        "Exit"));

            switch (choice)
            {
                case "Log Daily Expense":
                    LogExpense();
                    break;
                case "View Transaction History":
                    ShowTransactionHistory();
                    break;
                case "View Spending Breakdown":
                    ShowSpendingBreakdown();
                    break;
                case "Exit":
                    AnsiConsole.MarkupLine("[green]Goodbye.[/]");
                    return;
            }
        }
    }

    private void LogExpense()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]Log Daily Expense[/]").RuleStyle("grey").LeftJustified());

        var amount = PromptForAmount();
        var category = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Select a category[/]")
                .PageSize(10)
                .AddChoices(Categories));
        var date = PromptForDate();
        var description = AnsiConsole.Ask<string>("[yellow]Enter a short description:[/]").Trim();

        while (string.IsNullOrWhiteSpace(description))
        {
            AnsiConsole.MarkupLine("[red]Description cannot be empty.[/]");
            description = AnsiConsole.Ask<string>("[yellow]Enter a short description:[/]").Trim();
        }

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

        AnsiConsole.MarkupLine("\n[green]Expense saved successfully.[/]");
        var panel = new Panel($"{transaction.Date:yyyy-MM-dd} | {transaction.Category} | ${transaction.Amount:F2} | {transaction.Description}")
            .Header("Saved Entry")
            .Border(BoxBorder.Rounded);
        AnsiConsole.Write(panel);
        Pause();
    }

    private void ShowTransactionHistory()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]Transaction History[/]").RuleStyle("grey").LeftJustified());

        if (_transactions.Count == 0)
        {
            ShowMessage("No transactions found.");
            return;
        }

        var orderedTransactions = _transactions
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.CreatedAt)
            .ToList();

        var table = new Table().Border(TableBorder.Rounded).AddColumn("Date").AddColumn("Category").AddColumn("Amount").AddColumn("Description");

        foreach (var transaction in orderedTransactions)
        {
            table.AddRow(
                transaction.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                transaction.Category,
                $"[green]${transaction.Amount:F2}[/]",
                Markup.Escape(transaction.Description));
        }

        AnsiConsole.Write(table);
        Pause();
    }

    private void ShowSpendingBreakdown()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]Spending Breakdown[/]").RuleStyle("grey").LeftJustified());

        if (_transactions.Count == 0)
        {
            ShowMessage("No transactions found.");
            return;
        }

        var groups = _transactions
            .GroupBy(t => t.Category)
            .Select(group => new SpendingSummary(group.Key, group.Count(), group.Sum(t => t.Amount)))
            .OrderByDescending(group => group.Total)
            .ToList();

        var grandTotal = groups.Sum(group => group.Total);
        var table = new Table().Border(TableBorder.Rounded).AddColumn("Category").AddColumn("Entries").AddColumn("Total");

        foreach (var group in groups)
        {
            table.AddRow(
                group.Category,
                group.Count.ToString(CultureInfo.InvariantCulture),
                $"[green]${group.Total:F2}[/]");
        }

        AnsiConsole.Write(table);
        AnsiConsole.Write(
            new Panel($"[bold]Monthly Total:[/] [green]${grandTotal:F2}[/]")
                .Border(BoxBorder.Rounded)
                .Header("Summary"));

        Pause();
    }

    private static decimal PromptForAmount()
    {
        while (true)
        {
            var input = AnsiConsole.Ask<string>("[yellow]Enter amount:[/] $").Trim();
            if (decimal.TryParse(input, out var amount) && amount > 0)
            {
                return decimal.Round(amount, 2);
            }

            AnsiConsole.MarkupLine("[red]Please enter a valid positive amount.[/]");
        }
    }

    private static DateOnly PromptForDate()
    {
        while (true)
        {
            var input = AnsiConsole.Ask<string>("[yellow]Enter date (YYYY-MM-DD) or press Enter for today:[/]").Trim();
            if (string.IsNullOrWhiteSpace(input))
            {
                return DateOnly.FromDateTime(DateTime.Today);
            }

            if (DateOnly.TryParse(input, out var date))
            {
                return date;
            }

            AnsiConsole.MarkupLine("[red]Please enter a valid date in YYYY-MM-DD format.[/]");
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

    private static void ShowHeader()
    {
        AnsiConsole.Write(
            new FigletText("Financial Control")
                .LeftJustified()
                .Color(Color.Green));
        AnsiConsole.MarkupLine("[grey]A simple expense tracker for Daniel[/]\n");
    }

    private static void Pause()
    {
        AnsiConsole.MarkupLine("\n[grey]Press Enter to continue...[/]");
        Console.ReadLine();
    }

    private static void ShowMessage(string message)
    {
        AnsiConsole.MarkupLine($"[yellow]{Markup.Escape(message)}[/]");
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

internal sealed record SpendingSummary(string Category, int Count, decimal Total);
