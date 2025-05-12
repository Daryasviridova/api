using api.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.Extensions.Hosting;

namespace api.infrastructure
{
    public class Job : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public Job(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CurrencyDbContext>();
                var httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();

                // Удалить предыдущие данные
                dbContext.CurrencyRates.RemoveRange(dbContext.CurrencyRates);
                dbContext.Currencies.RemoveRange(dbContext.Currencies);
                await dbContext.SaveChangesAsync();

                var usdCurrency = new Currency { Name = "USD" };
                var eurCurrency = new Currency { Name = "EUR" };
                await dbContext.Currencies.AddRangeAsync(usdCurrency, eurCurrency);
                await dbContext.SaveChangesAsync();

                while (!stoppingToken.IsCancellationRequested)
                {
                    var json = await httpClient.GetFromJsonAsync<RatesResponse>("https://www.cbr-xml-daily.ru/daily_json.js");

                    dbContext.CurrencyRates.Add(new CurrencyRate
                    {
                        CurrencyId = usdCurrency.Id,
                        Rate = json.Valute.USD.Value,
                        Date = DateTime.UtcNow
                    });
                    dbContext.CurrencyRates.Add(new CurrencyRate
                    {
                        CurrencyId = eurCurrency.Id,
                        Rate = json.Valute.EUR.Value,
                        Date = DateTime.UtcNow
                    });


                    await dbContext.SaveChangesAsync();
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
    }
}

