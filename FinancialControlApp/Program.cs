using FinancialControlApp;
using FinancialControlApp.Persistence;
using FinancialControlApp.Services;

var dataStore = new JsonDataStore();
var transactionService = new TransactionService(dataStore);
var budgetService = new BudgetService(dataStore, transactionService);
var app = new AppShell(transactionService, budgetService);

app.Run();
