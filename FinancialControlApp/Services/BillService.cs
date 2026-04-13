using FinancialControlApp.Models;
using FinancialControlApp.Persistence;

namespace FinancialControlApp.Services;

internal sealed class BillService(JsonDataStore dataStore)
{
    public Bill AddBill(string name, decimal amount, DateOnly dueDate)
    {
        var bills = dataStore.LoadBills();
        var bill = new Bill
        {
            Name = name,
            Amount = amount,
            DueDate = dueDate
        };

        bills.Add(bill);
        dataStore.SaveBills(bills);
        return bill;
    }

    public IReadOnlyList<Bill> GetUpcomingBills()
    {
        return dataStore.LoadBills()
            .OrderBy(bill => bill.DueDate)
            .ThenBy(bill => bill.CreatedAt)
            .ToList();
    }

    public string GetBillStatusLabel(Bill bill, DateOnly? today = null)
    {
        var currentDate = today ?? DateOnly.FromDateTime(DateTime.Today);

        if (bill.DueDate < currentDate)
        {
            return "Overdue";
        }

        if (bill.DueDate <= currentDate.AddDays(7))
        {
            return "Due soon";
        }

        return "Upcoming";
    }
}
