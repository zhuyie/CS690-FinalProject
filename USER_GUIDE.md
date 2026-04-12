# User Guide

This guide explains how to use the main features of the Financial Control application.

## Main Features

- Log a daily expense
- View saved transaction history
- View spending totals by category

## Start The App

Run the following command from the repository root:

```bash
dotnet run --project FinancialControlApp/FinancialControlApp.csproj
```

## Main Menu

After the application starts, choose one of these options:

1. `Log Daily Expense`
2. `View Transaction History`
3. `View Spending Breakdown`
4. `Exit`

## How To Log A Daily Expense

1. Select `1` from the main menu.
2. Enter the expense amount.
3. Select a category from the numbered list.
4. Enter a date in `YYYY-MM-DD` format, or press Enter to use today.
5. Enter a short description.
6. Confirm that the expense was saved.

Example:

- Amount: `6.50`
- Category: `Coffee`
- Date: `2026-04-12`
- Description: `Morning latte`

## How To View Transaction History

1. Select `2` from the main menu.
2. Review the list of saved transactions.
3. The application shows transactions in reverse chronological order.

The history view includes:

- Date
- Category
- Amount
- Description

## How To View Spending Breakdown

1. Select `3` from the main menu.
2. Review the grouped spending summary.

The spending breakdown shows:

- Category name
- Number of entries in that category
- Total spending for that category
- Overall total spending

## Example Use Scenario

1. Log a coffee purchase.
2. Log a breakfast purchase.
3. Open transaction history to verify both entries were saved.
4. Open spending breakdown to see totals by category.

## Notes

- Transactions are saved locally between runs.
- If no transactions have been entered yet, the history and summary screens will show a message instead of data.
