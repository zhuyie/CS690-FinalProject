using FinancialControlApp.Models;
using FinancialControlApp.Services;
using Spectre.Console;

namespace FinancialControlApp;

internal sealed class AppShell(TransactionService transactionService, BudgetService budgetService)
{
    public void Run()
    {
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
                        "Manage Budgets",
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
                case "Manage Budgets":
                    ManageBudgets();
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
                .AddChoices(TransactionService.DefaultCategories));
        var date = PromptForDate();
        var description = PromptForRequiredText("[yellow]Enter a short description:[/]");

        var transaction = transactionService.AddTransaction(amount, category, date, description);

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

        var transactions = transactionService.GetTransactions();
        if (transactions.Count == 0)
        {
            ShowMessage("No transactions found.");
            return;
        }

        var table = new Table().Border(TableBorder.Rounded).AddColumn("Date").AddColumn("Category").AddColumn("Amount").AddColumn("Description");

        foreach (var transaction in transactions)
        {
            table.AddRow(
                transaction.Date.ToString("yyyy-MM-dd"),
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

        var summaries = transactionService.GetSpendingSummaries();
        if (summaries.Count == 0)
        {
            ShowMessage("No transactions found.");
            return;
        }

        var table = new Table().Border(TableBorder.Rounded).AddColumn("Category").AddColumn("Entries").AddColumn("Total");

        foreach (var summary in summaries)
        {
            table.AddRow(
                summary.Category,
                summary.Count.ToString(),
                $"[green]${summary.Total:F2}[/]");
        }

        var grandTotal = summaries.Sum(summary => summary.Total);
        AnsiConsole.Write(table);
        AnsiConsole.Write(
            new Panel($"[bold]Monthly Total:[/] [green]${grandTotal:F2}[/]")
                .Border(BoxBorder.Rounded)
                .Header("Summary"));

        Pause();
    }

    private static decimal PromptForAmount()
    {
        return PromptForAmount("[yellow]Enter amount:[/] $");
    }

    private static decimal PromptForAmount(string prompt)
    {
        while (true)
        {
            var input = AnsiConsole.Ask<string>(prompt).Trim();
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
            var input = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]Enter date (YYYY-MM-DD) or press Enter for today:[/]")
                    .AllowEmpty())
                .Trim();
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

    private static string PromptForRequiredText(string prompt)
    {
        while (true)
        {
            var input = AnsiConsole.Ask<string>(prompt).Trim();
            if (!string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            AnsiConsole.MarkupLine("[red]Description cannot be empty.[/]");
        }
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

    private void ManageBudgets()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[yellow]Manage Budgets[/]").RuleStyle("grey").LeftJustified());

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Budget options[/]")
                    .AddChoices("Set Monthly Limit", "View Budget Status", "Back"));

            switch (choice)
            {
                case "Set Monthly Limit":
                    SetMonthlyLimit();
                    break;
                case "View Budget Status":
                    ShowBudgetStatus();
                    break;
                case "Back":
                    return;
            }
        }
    }

    private void SetMonthlyLimit()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]Set Monthly Limit[/]").RuleStyle("grey").LeftJustified());

        var category = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Select a category[/]")
                .PageSize(10)
                .AddChoices(TransactionService.DefaultCategories));
        var monthlyLimit = PromptForAmount("[yellow]Enter monthly limit:[/] $");

        var budget = budgetService.SetBudget(category, monthlyLimit);

        AnsiConsole.MarkupLine($"\n[green]Budget saved.[/] {Markup.Escape(budget.Category)} limit is [green]${budget.MonthlyLimit:F2}[/].");
        Pause();
    }

    private void ShowBudgetStatus()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]Budget Status[/]").RuleStyle("grey").LeftJustified());

        var statuses = budgetService.GetBudgetStatuses();
        if (statuses.Count == 0)
        {
            ShowMessage("No budgets found. Add a monthly limit first.");
            return;
        }

        var table = new Table().Border(TableBorder.Rounded)
            .AddColumn("Category")
            .AddColumn("Limit")
            .AddColumn("Spent")
            .AddColumn("Remaining")
            .AddColumn("Status");

        foreach (var status in statuses)
        {
            table.AddRow(
                status.Category,
                $"${status.MonthlyLimit:F2}",
                $"${status.Spent:F2}",
                $"${status.Remaining:F2}",
                Markup.Escape(status.StatusLabel));
        }

        AnsiConsole.Write(table);
        Pause();
    }
}
