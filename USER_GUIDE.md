# User Guide

This guide explains how to use the main features of the Financial Control application.

## Main Features

- Log a daily expense
- View saved transaction history
- View spending totals by category
- Set monthly category budgets
- View budget status
- Add bills with due dates
- View upcoming bills

## Start The App

Run the following command from the repository root:

```bash
dotnet run --project FinancialControlApp/FinancialControlApp.csproj
```

## Main Menu

After the application starts, use the keyboard to move through the menu and press Enter to select an option:

1. `Log Daily Expense`
2. `View Transaction History`
3. `View Spending Breakdown`
4. `Manage Budgets`
5. `Track Bills`
6. `Exit`

## How To Log A Daily Expense

1. Choose `Log Daily Expense` from the main menu and press Enter.
2. Enter the expense amount.
3. Choose a category from the selection menu.
4. Enter a date in `YYYY-MM-DD` format, or press Enter to use today.
5. Enter a short description.
6. Confirm that the expense was saved.

Example:

- Amount: `6.50`
- Category: `Coffee`
- Date: `2026-04-12`
- Description: `Morning latte`

## How To View Transaction History

1. Choose `View Transaction History` from the main menu and press Enter.
2. Review the list of saved transactions.
3. The application shows transactions in reverse chronological order.

The history view includes:

- Date
- Category
- Amount
- Description

## How To View Spending Breakdown

1. Choose `View Spending Breakdown` from the main menu and press Enter.
2. Review the grouped spending summary.

The spending breakdown shows:

- Category name
- Number of entries in that category
- Total spending for that category
- Overall total spending

## How To Manage Budgets

1. Choose `Manage Budgets` from the main menu.
2. Select `Set Monthly Limit` to create or update a budget.
3. Choose a category from the selection menu.
4. Enter the monthly budget amount.
5. Select `View Budget Status` to review progress.

The budget status view includes:

- Category
- Monthly limit
- Amount spent
- Remaining amount
- Status: `On track`, `Near limit`, or `Over limit`

## How To Track Bills

1. Choose `Track Bills` from the main menu.
2. Select `Add Bill` to create a new bill.
3. Enter the bill name.
4. Enter the bill amount.
5. Enter the due date in `YYYY-MM-DD` format.
6. Select `View Upcoming Bills` to review saved bills.

The bills view includes:

- Bill name
- Due date
- Amount
- Status: `Upcoming`, `Due soon`, or `Overdue`

## Example Use Scenario

1. Log a coffee purchase.
2. Set a monthly coffee budget.
3. Add an upcoming rent bill.
4. Open transaction history to verify entries were saved.
5. Open spending breakdown to see totals by category.
6. Review budget status.
7. Review upcoming bills.

## Notes

- Transactions, budgets, and bills are saved locally between runs.
- If no data has been entered yet, the related views will show a message instead of data.
