using FinancialControlApp.Models;
using FinancialControlApp.Services;
using Spectre.Console;

namespace FinancialControlApp;

internal sealed class AppShell(TransactionService transactionService, BudgetService budgetService, BillService billService)
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
                        "Track Bills",
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
                case "Track Bills":
                    TrackBills();
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
        return PromptForDate("[yellow]Enter date (YYYY-MM-DD) or press Enter for today:[/]", allowEmpty: true);
    }

    private static DateOnly PromptForDate(string prompt, bool allowEmpty)
    {
        while (true)
        {
            var textPrompt = new TextPrompt<string>(prompt);
            if (allowEmpty)
            {
                textPrompt.AllowEmpty();
            }

            var input = AnsiConsole.Prompt(textPrompt).Trim();
            if (string.IsNullOrWhiteSpace(input) && allowEmpty)
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

    private void TrackBills()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[yellow]Track Bills[/]").RuleStyle("grey").LeftJustified());

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Bill options[/]")
                    .AddChoices("Add Bill", "View Upcoming Bills", "Back"));

            switch (choice)
            {
                case "Add Bill":
                    AddBill();
                    break;
                case "View Upcoming Bills":
                    ShowUpcomingBills();
                    break;
                case "Back":
                    return;
            }
        }
    }

    private void AddBill()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]Add Bill[/]").RuleStyle("grey").LeftJustified());

        var name = PromptForRequiredText("[yellow]Enter bill name:[/]");
        var amount = PromptForAmount();
        var dueDate = PromptForDate("[yellow]Enter due date (YYYY-MM-DD):[/]", allowEmpty: false);

        var bill = billService.AddBill(name, amount, dueDate);

        AnsiConsole.MarkupLine($"\n[green]Bill saved.[/] {Markup.Escape(bill.Name)} due on [green]{bill.DueDate:yyyy-MM-dd}[/].");
        Pause();
    }

    private void ShowUpcomingBills()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]Upcoming Bills[/]").RuleStyle("grey").LeftJustified());

        var bills = billService.GetUpcomingBills();
        if (bills.Count == 0)
        {
            ShowMessage("No upcoming bills found.");
            return;
        }

        var table = new Table().Border(TableBorder.Rounded)
            .AddColumn("Name")
            .AddColumn("Due Date")
            .AddColumn("Amount")
            .AddColumn("Status");

        foreach (var bill in bills)
        {
            table.AddRow(
                Markup.Escape(bill.Name),
                bill.DueDate.ToString("yyyy-MM-dd"),
                $"[green]${bill.Amount:F2}[/]",
                Markup.Escape(billService.GetBillStatusLabel(bill)));
        }

        AnsiConsole.Write(table);
        Pause();
    }
}
