# CS690-FinalProject

Iteration 1 delivers a working console application for Daniel's personal expense tracking.

## Documentation

- [Deployment Guide](DEPLOYMENT.md)
- [User Guide](USER_GUIDE.md)

## Release

- Iteration releases are published on GitHub Releases with downloadable binaries attached.
- Tagged versions such as `v0.1.1` are intended to produce packaged release artifacts automatically.

## Repository Structure

- `CS690-FinalProject.sln` - top-level solution file
- `FinancialControlApp` - main console application
- `images` - wiki and design assets

## Implemented Functional Requirements

- `FR1` Manual Transaction Entry
- `FR2` Category Selection
- `FR3` Transaction History List
- `FR4` Spending Summary by Category

## What The App Can Do

- Record daily expenses with amount, category, date, and description
- Save transactions between runs using a local JSON file
- Show transaction history in reverse chronological order
- Summarize spending totals by category

## Run The App

```bash
dotnet run --project FinancialControlApp/FinancialControlApp.csproj
```

## Project Structure

- `FinancialControlApp/Program.cs` - console application and core workflow
- `FinancialControlApp/FinancialControlApp.csproj` - .NET console project file
- `FinancialControlApp/data/transactions.json` - generated local storage file after first saved expense
