using FinancialControlApp;
using FinancialControlApp.Persistence;
using FinancialControlApp.Services;

var dataStore = new JsonDataStore();
var transactionService = new TransactionService(dataStore);
var budgetService = new BudgetService(dataStore, transactionService);
var billService = new BillService(dataStore);
var settingsService = new SettingsService(dataStore);
var alertService = new AlertService(budgetService, billService, settingsService);
var app = new AppShell(transactionService, budgetService, billService, settingsService, alertService);

app.Run();
