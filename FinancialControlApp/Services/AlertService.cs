namespace FinancialControlApp.Services;

internal sealed class AlertService(BudgetService budgetService, BillService billService, SettingsService settingsService)
{
    public IReadOnlyList<string> GetAlerts(DateOnly? today = null)
    {
        var currentDate = today ?? DateOnly.FromDateTime(DateTime.Today);
        var alerts = new List<string>();
        var bills = billService.GetUpcomingBills();

        foreach (var bill in bills)
        {
            var status = billService.GetBillStatusLabel(bill, currentDate);

            if (status == "Overdue")
            {
                alerts.Add($"{bill.Name} is overdue.");
            }
            else if (status == "Due soon")
            {
                var days = bill.DueDate.DayNumber - currentDate.DayNumber;
                alerts.Add($"{bill.Name} is due in {days} day{(days == 1 ? string.Empty : "s")}.");
            }
        }

        foreach (var budgetStatus in budgetService.GetBudgetStatuses())
        {
            if (budgetStatus.StatusLabel == "Over limit")
            {
                alerts.Add($"{budgetStatus.Category} budget exceeded by ${Math.Abs(budgetStatus.Remaining):F2}.");
            }
            else if (budgetStatus.StatusLabel == "Near limit")
            {
                alerts.Add($"{budgetStatus.Category} budget is near its limit.");
            }
        }

        var availableBalance = settingsService.GetAvailableBalance();
        if (availableBalance.HasValue)
        {
            var totalUpcomingBills = bills
                .Where(bill => bill.DueDate >= currentDate)
                .Sum(bill => bill.Amount);
            if (totalUpcomingBills > availableBalance.Value)
            {
                alerts.Add($"Upcoming bills total ${totalUpcomingBills:F2}, which exceeds available balance ${availableBalance.Value:F2}.");
            }
        }

        return alerts;
    }
}
