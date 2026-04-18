using FinancialControlApp.Models;
using FinancialControlApp.Persistence;

namespace FinancialControlApp.Services;

internal sealed class SettingsService(JsonDataStore dataStore)
{
    public decimal? GetAvailableBalance()
    {
        return dataStore.LoadSettings().AvailableBalance;
    }

    public void SetAvailableBalance(decimal amount)
    {
        var settings = dataStore.LoadSettings();
        settings.AvailableBalance = amount;
        dataStore.SaveSettings(settings);
    }
}
