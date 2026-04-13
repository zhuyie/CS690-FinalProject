using FinancialControlApp;
using FinancialControlApp.Persistence;
using FinancialControlApp.Services;

var dataStore = new JsonDataStore();
var transactionService = new TransactionService(dataStore);
var app = new AppShell(transactionService);

app.Run();
