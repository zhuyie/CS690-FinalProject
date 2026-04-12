# Deployment Guide

This guide explains how to download and run the Iteration 1 version of the Financial Control application.

## Preferred Download Option

If a GitHub Release is available, download the packaged binary for your platform from the repository's **Releases** page.

If you prefer to run from source, follow the steps below.

## Requirements

- .NET 10 SDK installed
- Git installed

## Clone The Repository

```bash
git clone https://github.com/zhuyie/CS690-FinalProject.git
cd CS690-FinalProject
```

## Project Location

The runnable application is located in:

```text
FinancialControlApp/
```

## Run The Application

From the repository root, run:

```bash
dotnet run --project FinancialControlApp/FinancialControlApp.csproj
```

## Alternative Run Steps

If you prefer, you can also run the app by moving into the project directory first:

```bash
cd FinancialControlApp
dotnet run
```

## Expected Behavior

When the application starts, you should see a console menu with these options:

1. Log Daily Expense
2. View Transaction History
3. View Spending Breakdown
4. Exit

## Data Storage

The application stores saved transactions locally in:

```text
FinancialControlApp/data/transactions.json
```

This file is created automatically after the first saved expense.

## Troubleshooting

### `dotnet: command not found`

Install the .NET 10 SDK and then rerun the command.

### Project does not start

Make sure you are running the command from the repository root, or from inside the `FinancialControlApp` directory.

### No transactions appear

This is expected until at least one expense has been entered and saved.
