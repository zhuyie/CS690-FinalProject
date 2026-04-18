# Deployment Guide

This guide explains how to download and run the current version of the Financial Control application.

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
4. Manage Budgets
5. Track Bills
6. Update Available Balance
7. View Critical Alerts
8. Exit

## Data Storage

The application stores saved data locally in:

```text
FinancialControlApp/data/transactions.json
FinancialControlApp/data/budgets.json
FinancialControlApp/data/bills.json
FinancialControlApp/data/settings.json
```

These files are created automatically after the related data is saved for the first time.

## Run Tests

From the repository root, run:

```bash
dotnet test CS690-FinalProject.sln
```

## Troubleshooting

### `dotnet: command not found`

Install the .NET 10 SDK and then rerun the command.

### macOS says Apple could not verify the app

The packaged macOS binary is not code-signed or notarized, so macOS Gatekeeper may block it the first time.

You can still open it by using one of these methods:

#### Option 1: Open from Finder

1. Locate the `FinancialControlApp` file in Finder.
2. Right-click the file.
3. Select `Open`.
4. Confirm by clicking `Open` again.

#### Option 2: Allow it in System Settings

1. Try opening the application once.
2. Open **System Settings > Privacy & Security**.
3. Find the blocked application message near the bottom.
4. Select **Open Anyway**.

#### Option 3: Remove the quarantine flag in Terminal

```bash
xattr -dr com.apple.quarantine FinancialControlApp
```

### Project does not start

Make sure you are running the command from the repository root, or from inside the `FinancialControlApp` directory.

### No transactions appear

This is expected until at least one expense has been entered and saved.
