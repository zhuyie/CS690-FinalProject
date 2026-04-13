using FinancialControlApp.Persistence;
using FinancialControlApp.Services;
using Xunit;

namespace FinancialControlApp.Tests;

public sealed class ServiceTests : IDisposable
{
    private readonly string _baseDirectory;

    public ServiceTests()
    {
        _baseDirectory = Path.Combine(Path.GetTempPath(), "financial-control-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_baseDirectory);
    }

    [Fact]
    public void GetSpendingSummaries_GroupsTransactionsByCategory()
    {
        var store = new JsonDataStore(_baseDirectory);
        var service = new TransactionService(store);

        service.AddTransaction(6.50m, "Coffee", new DateOnly(2026, 4, 13), "Morning latte");
        service.AddTransaction(3.50m, "Coffee", new DateOnly(2026, 4, 13), "Refill");
        service.AddTransaction(8.25m, "Breakfast", new DateOnly(2026, 4, 13), "Bagel");

        var summaries = service.GetSpendingSummaries();

        Assert.Equal(2, summaries.Count);
        Assert.Equal("Coffee", summaries[0].Category);
        Assert.Equal(10.00m, summaries[0].Total);
        Assert.Equal(2, summaries[0].Count);
    }

    [Fact]
    public void GetBudgetStatuses_ReturnsNearLimitWhenSpendingReachesEightyPercent()
    {
        var store = new JsonDataStore(_baseDirectory);
        var transactionService = new TransactionService(store);
        var budgetService = new BudgetService(store, transactionService);

        transactionService.AddTransaction(40m, "Groceries", new DateOnly(2026, 4, 13), "Trip 1");
        transactionService.AddTransaction(40m, "Groceries", new DateOnly(2026, 4, 14), "Trip 2");
        budgetService.SetBudget("Groceries", 100m);

        var status = budgetService.GetBudgetStatuses().Single();

        Assert.Equal("Near limit", status.StatusLabel);
        Assert.Equal(80m, status.Spent);
        Assert.Equal(20m, status.Remaining);
    }

    [Fact]
    public void GetBudgetStatuses_ReturnsOverLimitWhenSpendingExceedsBudget()
    {
        var store = new JsonDataStore(_baseDirectory);
        var transactionService = new TransactionService(store);
        var budgetService = new BudgetService(store, transactionService);

        transactionService.AddTransaction(70m, "Coffee", new DateOnly(2026, 4, 13), "Week 1");
        transactionService.AddTransaction(50m, "Coffee", new DateOnly(2026, 4, 14), "Week 2");
        budgetService.SetBudget("Coffee", 100m);

        var status = budgetService.GetBudgetStatuses().Single();

        Assert.Equal("Over limit", status.StatusLabel);
        Assert.Equal(120m, status.Spent);
        Assert.Equal(-20m, status.Remaining);
    }

    [Fact]
    public void SetBudget_UpdatesExistingBudgetInsteadOfCreatingDuplicate()
    {
        var store = new JsonDataStore(_baseDirectory);
        var transactionService = new TransactionService(store);
        var budgetService = new BudgetService(store, transactionService);

        budgetService.SetBudget("Transport", 50m);
        budgetService.SetBudget("Transport", 75m);

        var statuses = budgetService.GetBudgetStatuses();

        Assert.Single(statuses);
        Assert.Equal(75m, statuses[0].MonthlyLimit);
    }

    [Fact]
    public void GetUpcomingBills_ReturnsBillsSortedByDueDate()
    {
        var store = new JsonDataStore(_baseDirectory);
        var billService = new BillService(store);

        billService.AddBill("Internet", 60m, new DateOnly(2026, 4, 22));
        billService.AddBill("Rent", 900m, new DateOnly(2026, 4, 18));
        billService.AddBill("Utilities", 120m, new DateOnly(2026, 4, 20));

        var bills = billService.GetUpcomingBills();

        Assert.Equal("Rent", bills[0].Name);
        Assert.Equal("Utilities", bills[1].Name);
        Assert.Equal("Internet", bills[2].Name);
    }

    [Fact]
    public void GetBillStatusLabel_ReturnsDueSoonForBillsWithinSevenDays()
    {
        var store = new JsonDataStore(_baseDirectory);
        var billService = new BillService(store);
        var bill = billService.AddBill("Rent", 900m, new DateOnly(2026, 4, 18));

        var status = billService.GetBillStatusLabel(bill, new DateOnly(2026, 4, 13));

        Assert.Equal("Due soon", status);
    }

    [Fact]
    public void GetBillStatusLabel_ReturnsOverdueForPastBills()
    {
        var store = new JsonDataStore(_baseDirectory);
        var billService = new BillService(store);
        var bill = billService.AddBill("Utilities", 120m, new DateOnly(2026, 4, 10));

        var status = billService.GetBillStatusLabel(bill, new DateOnly(2026, 4, 13));

        Assert.Equal("Overdue", status);
    }

    [Fact]
    public void GetTransactions_ReturnsNewestTransactionsFirst()
    {
        var store = new JsonDataStore(_baseDirectory);
        var transactionService = new TransactionService(store);

        transactionService.AddTransaction(4m, "Coffee", new DateOnly(2026, 4, 10), "Old");
        transactionService.AddTransaction(9m, "Lunch", new DateOnly(2026, 4, 15), "New");

        var transactions = transactionService.GetTransactions();

        Assert.Equal("New", transactions[0].Description);
        Assert.Equal("Old", transactions[1].Description);
    }

    public void Dispose()
    {
        if (Directory.Exists(_baseDirectory))
        {
            Directory.Delete(_baseDirectory, true);
        }
    }
}
