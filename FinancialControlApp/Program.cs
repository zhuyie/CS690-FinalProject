using FinancialControlApp;
using FinancialControlApp.Persistence;
using FinancialControlApp.Services;

var dataStore = new JsonDataStore();
var transactionService = new TransactionService(dataStore);
var budgetService = new BudgetService(dataStore, transactionService);
var billService = new BillService(dataStore);
var app = new AppShell(transactionService, budgetService, billService);

app.Run();
